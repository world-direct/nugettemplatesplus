using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGetTemplatesPlus.Library;
using System.Xml.Serialization;
using System.IO;
using System.CodeDom;

namespace NuGetTemplatesPlus.Demo1 {

    public class DemoConfigCodeGenerator : CodeDomCodeGenerator {

        protected override void GenerateCode() {

            var democonfig = (democonfig)new XmlSerializer(typeof(democonfig)).Deserialize(new StringReader(this.InputFileContent));

            foreach (var configClass in democonfig.configClass) {
                var codeClass = new CodeTypeDeclaration(configClass.name);
                this.TargetNamespace.Types.Add(codeClass);

                foreach (var configProperty in configClass.configProperty) {
                    var codeProperty = new CodeMemberProperty();
                    codeClass.Members.Add(codeProperty);
                    codeProperty.Name = configProperty.name;
                    codeProperty.Type = new CodeTypeReference(typeof(string));
                    codeProperty.HasGet = true;
                    codeProperty.GetStatements.Add(
                        new CodeMethodReturnStatement(
                            new CodePrimitiveExpression("not implemented")
                        )
                    );
                }
            }
        }
    }
}
