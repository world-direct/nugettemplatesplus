using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace NuGetTemplatesPlus.Library {

    /// <summary>
    /// Inherit from this class to imlement a CodeGenerator based on System.CodeDom.
    /// You have to use the <see cref="InputFileContent"/> property to populate the <see cref="TargetNamespace"/> instance
    /// in the <see cref="GenerateCode"/> method.
    /// The class must have a public parameterless constructor.
    /// </summary>
    public abstract class CodeDomCodeGenerator : MarshalByRefObject, Interface.ICodeGenerator {

        Interface.ICodeGeneratorContext Context { get; set; }

        /// <summary>
        /// Use the <see cref="InputFileContent"/> property to populate the <see cref="TargetNamespace"/> instance in this method        
        /// </summary>
        protected abstract void GenerateCode();

        /// <summary>
        /// Contains a reference to the initialized CodeNamespace that is the target of the generation.
        /// </summary>
        public CodeNamespace TargetNamespace { get; private set; }

        /// <summary>
        /// Allows access to the options used during generation.
        /// </summary>
        public CodeGeneratorOptions CodeGeneratorOptions { get; private set; }

        /// <summary>
        /// Returns the content of the input file.
        /// </summary>
        public string InputFileContent { get; private set; }

        #region ICodeGenerator Members

        byte[] Interface.ICodeGenerator.GenerateCode(Interface.ICodeGeneratorContext args) {
            this.Context = args;
            this.TargetNamespace = new CodeNamespace(args.FileNameSpace);
            this.CodeGeneratorOptions = new System.CodeDom.Compiler.CodeGeneratorOptions();
            this.InputFileContent = args.InputFileContent;
            GenerateCode();

            using (var stream = new MemoryStream()) {
                using (var writer = new StreamWriter(stream)) {
                    args.CodeDomProvider.GenerateCodeFromNamespace(this.TargetNamespace, writer, this.CodeGeneratorOptions);
                    writer.Flush();
                    return stream.ToArray();
                }
            }

        }

        #endregion
    }
}
