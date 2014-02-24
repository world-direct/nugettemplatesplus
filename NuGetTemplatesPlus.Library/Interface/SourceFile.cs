using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetTemplatesPlus.Library.Interface {

    /// <summary>
    /// Represents a file in the target-project
    /// </summary>
    public interface ISourceFile {

        /// <summary>
        /// The path to the Item
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// The content of the Item
        /// </summary>
        string FileContent { get; }

    }

    /// <summary>
    /// Represents a file in the process of code-generation with the 'NuGetTemplatesPlus" Custom-Tool
    /// </summary>
    public interface ICustomToolSourceFile : ISourceFile {

        /// <summary>
        /// Returns the CodeDomProvider for the file
        /// </summary>
        System.CodeDom.Compiler.CodeDomProvider CodeDomProvider { get; }

        /// <summary>
        /// Returns the name of the namespace
        /// </summary>
        string FileNameSpace { get; }
    }

}