using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using NuGetTemplatesPlus.Library.Interface;

namespace NuGetTemplatesPlus.Engine {
    public abstract class ProjectItemInfo {

        public abstract string FullName { get; }
        public abstract ProjectInfo Project { get; }

        public CustomToolInfo GetCustomToolInfo(ISourceFile sourceFile) {
            return this.Project.CustomTools.Where(c => c.Condition.Evaluate(sourceFile)).SingleOrDefault();
        }     

        public IEnumerable<XmlSchemaInfo> GetXmlSchemaInfos(ISourceFile sourceFile) {
            return this.Project.XmlSchemas.Where(c => c.Condition.Evaluate(sourceFile));
        }

    }
}
