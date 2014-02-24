using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetTemplatesPlus.Library.Interface {

    /// <summary>
    /// Low-Level interface for Code-Generation
    /// </summary>
    public interface ICustomTool {

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        /// <param name="sourceFile">the input-file</param>
        /// <param name="parameter">the (optional) parameter, as specified in the NuGetTemplates.xml configuration file</param>
        /// <returns></returns>
        byte[] GenerateCode(ICustomToolSourceFile sourceFile, string parameter);
    }
}
