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


namespace GProssliner.NuGetTemplatesPlus_VSPackage{

    [ComVisible(true)]
    [Guid("A95A46D0-3CC8-4BA5-9578-A1BD9184E17C")]
    [CodeGeneratorRegistration(typeof(CustomTool), "nugettempl", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, GeneratorRegKeyName = "nugettempl")]
    [ProvideObject(typeof(CustomTool))]
    [ClassInterface(ClassInterfaceType.None)]
    public class CustomTool : BaseCodeGeneratorWithSite, ICodeGeneratorContext {


        protected override byte[] GenerateCode(string inputFileContent) {

            var projectItem = new EnvDTEProjectItemInfo(this.GetProjectItem());
            var codeGeneratorType = projectItem.TryGetCodeGeneratorType();
            if (codeGeneratorType == null) {
                this.GeneratorError(0, "NO CODEGENERATOR-CLASS!", 0, 0);
                return new byte[0];
            }

            var codeGenerator = (ICodeGenerator)Activator.CreateInstance(codeGeneratorType);
            this.InputFileContent = inputFileContent;
            var content = codeGenerator.GenerateCode(this);

            return content;


        }

        #region ICodeGeneratorContext Members

        public string InputFileContent {
            get;
            private set;
        }


        System.CodeDom.Compiler.CodeDomProvider ICodeGeneratorContext.CodeDomProvider {
            get { return this.GetCodeProvider(); }
        }

        #endregion
    }
}
