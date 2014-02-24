using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGetTemplatesPlus.Library.Interface;

namespace NuGetTemplatesPlus.Engine {

    class T4CustomTool : ICustomTool {

        #region ICustomTool Members

        public byte[] GenerateCode(ICustomToolSourceFile sourceFile, string parameter) {

            throw new NotImplementedException();
            Type templatedProcessor = Type.GetTypeFromCLSID(new Guid("F56DB4B6-C280-40f1-855D-5DA0ED7BD270"));


        }

        #endregion
    }
}
