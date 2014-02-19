using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

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

        public IEnumerable<string> GetItemTemplates(string language) {
            var path = "ItemTemplates\\" + language;

            return this.EnumerateNuGetTemplatesPlusModuleFiles(path, "*.zip")
                .ToList();
        }
    }
}
