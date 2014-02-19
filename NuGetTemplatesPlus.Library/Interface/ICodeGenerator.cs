using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetTemplatesPlus.Library.Interface {

    /// <summary>
    /// Low-Level interface for Code-Generation
    /// </summary>
    public interface ICodeGenerator {

        /// <summary>
        /// Low-Level interface for Code-Generation
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        byte[] GenerateCode(ICodeGeneratorContext args);
    }
}
