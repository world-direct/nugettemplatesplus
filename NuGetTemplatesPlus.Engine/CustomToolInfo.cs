using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGetTemplatesPlus.Library.Interface;
using System.Xml;

namespace NuGetTemplatesPlus.Engine {

    public class CustomToolInfo {
        public SourceFileConditionInfo Condition;
        public string TypeName;
        public string Parameters;
    }

}