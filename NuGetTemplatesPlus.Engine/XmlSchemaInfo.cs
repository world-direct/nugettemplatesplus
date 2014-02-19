using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.IO;

namespace NuGetTemplatesPlus.Engine {

    public class XmlSchemaInfo {

        public string TargetNamespace;
        public string XsdFilePath;

        public XmlSchemaInfo(string xsdFilePath) {
            this.XsdFilePath = xsdFilePath;

            using (var fs = File.OpenRead(xsdFilePath)) {
                var schema = XmlSchema.Read(fs, null);
                this.TargetNamespace = schema.TargetNamespace;
            }
        }
    }
}
