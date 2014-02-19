using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NuGetTemplatesPlus.Engine {

    public class PackageInfo {

        public string Id;
        public string Version;
        public string Specification {
            get { return Id + "." + Version; }
        }

        internal NuGetTemplatePlusPackageInfo TryConvertToNuGetTemplatePlusPackageInfo(SolutionInfo solution) {

            var packagesFolder = Path.Combine(Path.GetDirectoryName(solution.FullName), "packages");

            var packageFolder = Path.Combine(packagesFolder, this.Specification);
            var packageToolsFolder = Path.Combine(packageFolder, "tools\\NuGetTemplatesPlus");

            if (!Directory.Exists(packageToolsFolder))
                return null;

            return new NuGetTemplatePlusPackageInfo(packageToolsFolder) {
                Id = this.Id,
                Version = this.Version
            };
        }
    }

}