using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.IO;
using NuGetTemplatesPlus.Library.Interface;

namespace NuGetTemplatesPlus.Engine {

    public class XmlSchemaInfo {

        public string TargetNamespace;
        public Uri Location;
        public SourceFileConditionInfo Condition;

        public XmlSchemaInfo(string xsdFilePath) {
            this.Location = new Uri(xsdFilePath);

            using (var fs = File.OpenRead(xsdFilePath)) {
                var schema = XmlSchema.Read(fs, null);
                this.TargetNamespace = schema.TargetNamespace;
            }
        }
    }
}
