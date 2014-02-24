using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuGetTemplatesPlus.Library.Interface {

    /// <summary>
    /// Implement this interface to allow custom conditions to be evaluated in NuGetTemplatesPlus.xml configuration files
    /// </summary>
    public interface ISourceFileCondition {

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="parameter">the (optional) parameter, as specified in the NuGetTemplates.xml configuration file</param>
        void Initialize(string parameter);

        /// <summary>
        /// Evaluates the condition, and returns 'true' if the condition is fullfilled.
        /// </summary>
        /// <param name="file">the file</param>
        /// <returns></returns>
        bool Evaluate(ISourceFile file);
    }

}