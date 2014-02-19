using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using NuGetTemplatesPlus.Engine;

namespace GProssliner.NuGetTemplatesPlus_VSPackage {

    class EnvDTEProjectInfo : ProjectInfo {

        Project _project;

        public EnvDTEProjectInfo(Project project) {
            _project = project;
        }

        public override string FullName {
            get { return _project.FullName; }
        }

        public override SolutionInfo Solution {
            get { return new EnvDTESolutionInfo(_project.DTE.Solution); }
        }
    }
}
