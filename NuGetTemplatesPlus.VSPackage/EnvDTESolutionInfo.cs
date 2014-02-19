using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using NuGetTemplatesPlus.Engine;

namespace GProssliner.NuGetTemplatesPlus_VSPackage {

    class EnvDTESolutionInfo : SolutionInfo {

        Solution _solution;

        public EnvDTESolutionInfo(Solution solution) {
            _solution = solution;
        }

        public override string FullName {
            get { return _solution.FullName; }
        }
    }

}
