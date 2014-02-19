using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using NuGetTemplatesPlus.Engine;

namespace GProssliner.NuGetTemplatesPlus_VSPackage {

    class EnvDTEProjectItemInfo : ProjectItemInfo {

        ProjectItem _projectItem;

        public EnvDTEProjectItemInfo(ProjectItem item) {
            _projectItem = item;
        }

        public override string FullName {
            get { return _projectItem.Name; }
        }

        public override ProjectInfo Project {
            get { return new EnvDTEProjectInfo(_projectItem.ContainingProject); }
        }
    }
}
