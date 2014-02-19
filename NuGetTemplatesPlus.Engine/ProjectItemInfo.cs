using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace NuGetTemplatesPlus.Engine {
    public abstract class ProjectItemInfo {
        public abstract string FullName { get; }
        public abstract ProjectInfo Project { get; }

        public Type TryGetCodeGeneratorType() {

            var extension = Path.GetExtension(this.FullName).TrimStart('.');
            var matchingGeneratorName = extension + "CodeGenerator";

            var dlls = this.Project.EnumerateNuGetTemplatesPlusModuleFiles("CodeGenerators", "*.dll");

            var matchingType = default(Type);
            foreach (var dllName in dlls) {
                var assembly = Assembly.LoadFrom(dllName);

                if (matchingType == null) {
                    matchingType = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && string.Compare(t.Name, matchingGeneratorName, true) == 0)
                    .SingleOrDefault();
                }
            }

            return matchingType;

        }

        public IEnumerable<XmlSchemaInfo> GetXmlSchemaInfos() {
            return this.Project.EnumerateNuGetTemplatesPlusModuleFiles("XmlSchemas", "*.xsd")
                .Select(p => new XmlSchemaInfo(p))
                .ToList();
        }

    }
}
