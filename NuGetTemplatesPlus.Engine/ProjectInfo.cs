using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;

namespace NuGetTemplatesPlus.Engine {

    public abstract class ProjectInfo {

        public abstract string FullName { get; }
        public abstract SolutionInfo Solution { get; }

        IEnumerable<PackageInfo> GetInstalledPackages() {
            var packagesConfigProject = Path.Combine(Path.GetDirectoryName(this.FullName), "packages.config");

            foreach (var projectLevel in ReadPackagesInfo(packagesConfigProject))
                yield return projectLevel;


            var packagesConfigSolution = Path.Combine(Path.GetDirectoryName(this.Solution.FullName), ".nuget\\packages.config");

            foreach (var solutionLevel in ReadPackagesInfo(packagesConfigSolution))
                yield return solutionLevel;

        }

        IEnumerable<PackageInfo> ReadPackagesInfo(string packagesConfigPath) {

            if (!File.Exists(packagesConfigPath))
                yield break;

            var packagesConfigDoc = new XmlDocument();
            packagesConfigDoc.Load(packagesConfigPath);

            foreach (var packageInfo in packagesConfigDoc.DocumentElement.SelectNodes("package").Cast<XmlElement>()
                .Select(e => new PackageInfo { Id = e.GetAttribute("id"), Version = e.GetAttribute("version") })) {

                var nuGetTemplatePlusPackageInfo = packageInfo.TryConvertToNuGetTemplatePlusPackageInfo(this.Solution);
                yield return nuGetTemplatePlusPackageInfo ?? packageInfo;

            }

        }

        IList<PackageInfo> _installedPackages;
        public IEnumerable<PackageInfo> InstalledPackages {
            get {
                if (_installedPackages == null)
                    _installedPackages = GetInstalledPackages().ToList();

                return _installedPackages;
            }
        }

        IEnumerable<NuGetTemplatePlusPackageInfo> GetRelevateNuGetTemplatePlusPackageInfos() {
            return this.InstalledPackages.OfType<NuGetTemplatePlusPackageInfo>()
                .Where(p => p.Dependencies.All(d => d.IsFullfilled(this)));
        }

        IList<NuGetTemplatePlusPackageInfo> _nuGetTemplatePlusPackage;
        public IEnumerable<NuGetTemplatePlusPackageInfo> NuGetTemplatePlusPackages {
            get {
                if (_nuGetTemplatePlusPackage == null)
                    _nuGetTemplatePlusPackage = GetRelevateNuGetTemplatePlusPackageInfos().ToList();

                return _nuGetTemplatePlusPackage;
            }
        }

        public IEnumerable<string> EnumerateNuGetTemplatesPlusModuleFiles(string relativePath, string pattern) {
            foreach (var nuGetTemplatePlusPackage in this.NuGetTemplatePlusPackages) {
                var absolutePath = Path.Combine(nuGetTemplatePlusPackage.NuGetTemplatesPlusFolder, relativePath);
                if (!Directory.Exists(absolutePath))
                    continue;

                foreach (var match in Directory.GetFiles(absolutePath, pattern))
                    yield return match;
            }
        }

        public IEnumerable<CustomToolInfo> CustomTools {
            get {
                return this.NuGetTemplatePlusPackages.SelectMany(p => p.CustomTools);
            }
        }

        public IEnumerable<XmlSchemaInfo> XmlSchemas {
            get {
                return this.NuGetTemplatePlusPackages.SelectMany(p => p.XmlSchemas);
            }
        }

        public IEnumerable<string> LibFolders {
            get {
                foreach (var nuGetTemplatePlusPackage in this.NuGetTemplatePlusPackages) {
                    var absolutePath = Path.Combine(nuGetTemplatePlusPackage.NuGetTemplatesPlusFolder, "Lib");
                    if (!Directory.Exists(absolutePath))
                        continue;

                    yield return absolutePath;
                }
            }
        }

        public IEnumerable<string> GetItemTemplates(string language) {
            var path = "ItemTemplates\\" + language;

            return this.EnumerateNuGetTemplatesPlusModuleFiles(path, "*.zip")
                .ToList();
        }


        public IDisposable AssemblyResolve() {
            return new AssemblyResolver(this.LibFolders);
        }

        class AssemblyResolver : IDisposable {

            string[] _paths;

            public AssemblyResolver(IEnumerable<string> paths) {
                _paths = paths.ToArray();
                AppDomain.CurrentDomain.AssemblyResolve += assembly_resolve;
            }

            Assembly assembly_resolve(object sender, ResolveEventArgs e) {
                foreach (var path in _paths) {
                    var dllPath = Path.Combine(path, e.Name + ".dll");
                    if (File.Exists(dllPath))
                        return Assembly.LoadFrom(dllPath);
                }

                return null;
            }

            #region IDisposable Members

            public void Dispose() {
                AppDomain.CurrentDomain.AssemblyResolve -= assembly_resolve;
            }

            #endregion
        }
    }
}
