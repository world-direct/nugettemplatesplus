using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace NuGetTemplatesPlus.Engine {

    public class NuGetTemplatePlusPackageInfo : PackageInfo {

        public string NuGetTemplatesPlusFolder { get; private set; }
        public IEnumerable<DependencyInfo> Dependencies { get; private set; }

        public NuGetTemplatePlusPackageInfo(string nuGetTemplatesPlusFolder) {
            this.NuGetTemplatesPlusFolder = nuGetTemplatesPlusFolder;

            var configFile = Path.Combine(this.NuGetTemplatesPlusFolder, "NuGetTemplatesPlus.xml");
            if (!File.Exists(configFile)) {
                this.Dependencies = Enumerable.Empty<DependencyInfo>();
            } else {

                var configXmlDoc = new XmlDocument();
                configXmlDoc.Load(configFile);

                this.Dependencies = configXmlDoc.SelectNodes("configuration/dependencies/dependency")
                    .OfType<XmlElement>()
                    .Select(e => e.GetAttribute("id"))
                    .Select(id => new DependencyInfo { PackageId = id })
                    .ToList();

            }

        }



    }
}
