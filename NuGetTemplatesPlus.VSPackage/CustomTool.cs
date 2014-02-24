using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSLangProj80;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using NuGetTemplatesPlus.Library.Interface;


namespace GProssliner.NuGetTemplatesPlus_VSPackage {

    [ComVisible(true)]
    [Guid("A95A46D0-3CC8-4BA5-9578-A1BD9184E17C")]
    [CodeGeneratorRegistration(typeof(CustomTool), "NuGetTemplatesPlus", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, GeneratorRegKeyName = "NuGetTemplatesPlus")]
    [ProvideObject(typeof(CustomTool))]
    [ClassInterface(ClassInterfaceType.None)]
    public class CustomTool : BaseCodeGeneratorWithSite {

        class CustomToolInputFile : ICustomToolSourceFile {

            #region ISourceFile Members

            public string FilePath {
                get;
                set;
            }

            public string FileContent {
                get;
                set;
            }

            public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
                get;
                set;
            }

            public string FileNameSpace {
                get;
                set;
            }

            #endregion
        }

        protected override byte[] GenerateCode(string inputFileContent) {

            var inputFile = new CustomToolInputFile {
                CodeDomProvider = this.GetCodeProvider(),
                FileContent = inputFileContent,
                FilePath = this.InputFilePath,
                FileNameSpace = this.FileNameSpace
            };

            var projectItem = new EnvDTEProjectItemInfo(this.GetProjectItem());
            using (projectItem.Project.AssemblyResolve()) {

                var customToolInfo = projectItem.GetCustomToolInfo(inputFile);

                if (customToolInfo == null) {
                    this.GeneratorError(0, "NO CODEGENERATOR-CLASS!", 0, 0);
                    return new byte[0];
                }


                var customToolType = Type.GetType(customToolInfo.TypeName);
                var customTool = (ICustomTool)Activator.CreateInstance(customToolType);
                var content = customTool.GenerateCode(inputFile, customToolInfo.Parameters);

                return content;

            }

        }

        protected override string GetDefaultExtension() {
            return ".designer" + base.GetDefaultExtension();
        }

    }
}
