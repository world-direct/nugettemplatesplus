using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGetTemplatesPlus.Library.Interface;

namespace NuGetTemplatesPlus.Engine {

    public class SourceFileConditionInfo {
        public string ConditionTypeName;
        public string ConditionParameter;

        public bool Evaluate(ISourceFile sourceFile) {
            var type = Type.GetType(this.ConditionTypeName);
            var condition = (ISourceFileCondition)Activator.CreateInstance(type);
            condition.Initialize(this.ConditionParameter);

            return condition.Evaluate(sourceFile);
        }

    }

}