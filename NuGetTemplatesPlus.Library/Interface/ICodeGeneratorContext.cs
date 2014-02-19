using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace NuGetTemplatesPlus.Library.Interface {

    /// <summary>
    /// Low-Level interface for Code-Generation
    /// </summary>
    public interface ICodeGeneratorContext {

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        string InputFileContent { get; }

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        string InputFilePath { get; }

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        string FileNameSpace { get; }

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        CodeDomProvider CodeDomProvider { get; }
    }
}