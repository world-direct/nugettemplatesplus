using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using NuGetTemplatesPlus.Library.Interface;

namespace NuGetTemplatesPlus.Engine {

    public class NuGetTemplatePlusPackageInfo : PackageInfo {

        public string NuGetTemplatesPlusFolder { get; private set; }
        public IEnumerable<DependencyInfo> Dependencies { get; private set; }
        public IEnumerable<CustomToolInfo> CustomTools { get; private set; }
        public IEnumerable<XmlSchemaInfo> XmlSchemas { get; private set; }

        public NuGetTemplatePlusPackageInfo(string nuGetTemplatesPlusFolder) {
            this.NuGetTemplatesPlusFolder = nuGetTemplatesPlusFolder;

            this.Dependencies = Enumerable.Empty<DependencyInfo>();
            this.CustomTools = Enumerable.Empty<CustomToolInfo>();
            this.XmlSchemas = Enumerable.Empty<XmlSchemaInfo>();

            var libPath = Path.Combine(nuGetTemplatesPlusFolder, "Lib");

            var configFile = Path.Combine(this.NuGetTemplatesPlusFolder, "NuGetTemplatesPlus.xml");
            if (File.Exists(configFile)) {
                using (var reader = new StreamReader(configFile)) {
                    var config = (nuGetTemplatesPlus)new XmlSerializer(typeof(nuGetTemplatesPlus)).Deserialize(reader);
                    InitConfiguration(config);
                }
            }

        }

        void InitConfiguration(nuGetTemplatesPlus config) {

            if (config.dependencies != null) {
                this.Dependencies = config.dependencies.Select(d => new DependencyInfo { PackageId = d.id });
            }

            foreach (var ruleBody in config.sourceFiles.Items) {

                var fileExtensionRule = ruleBody as nuGetTemplatesPlusSourceFilesIfFileExtension;
                var condition = new SourceFileConditionInfo();

                if (fileExtensionRule != null) {
                    condition.ConditionTypeName = typeof(FileExtensionCondition).AssemblyQualifiedName;
                    condition.ConditionParameter = fileExtensionRule.extension;
                }

                var customConditionRule = ruleBody as nuGetTemplatesPlusSourceFilesIfCustomCondition;
                if (customConditionRule != null) {
                    condition.ConditionTypeName = customConditionRule.type;
                    condition.ConditionParameter = customConditionRule.parameter;
                }

                this.CustomTools = ruleBody.Items.OfType<RuleBodyApplyCustomTool>().Select(ct => new CustomToolInfo {
                    Condition = condition,
                    TypeName = ct.type,
                    Parameters = ct.parameter
                }).ToList();

                this.XmlSchemas = ruleBody.Items.OfType<RuleBodyIncludeXmlSchema>().Select(ct => new XmlSchemaInfo(Path.Combine(this.NuGetTemplatesPlusFolder, ct.file)) {
                    Condition = condition,
                }).ToList();


            }

        }


        class FileExtensionCondition : ISourceFileCondition {

            string _extension;

            #region ISourceFileCondition Members

            public void Initialize(string parameter) {
                _extension = parameter;
            }

            public bool Evaluate(ISourceFile file) {
                return string.Compare(Path.GetExtension(file.FilePath), _extension, true) == 0;
            }

            #endregion
        }

    }



}
