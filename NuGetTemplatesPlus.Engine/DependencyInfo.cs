using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetTemplatesPlus.Engine {

    public class DependencyInfo {
        public string PackageId;

        public bool IsFullfilled(ProjectInfo project) {
            return project.InstalledPackages.Any(p => p.Id == PackageId);
        }
    }
}
