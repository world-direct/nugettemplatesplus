// Guids.cs
// MUST match guids.h
using System;

namespace GProssliner.NuGetTemplatesPlus_VSPackage
{
    static class GuidList
    {
        public const string guidNuGetTemplatesPlus_VSPackagePkgString = "cfba5541-f1e6-48e9-a5a0-daf5707099b2";
        public const string guidNuGetTemplatesPlus_VSPackageCmdSetString = "a706d349-053d-4eea-a0f9-57c367b0d6db";

        public static readonly Guid guidNuGetTemplatesPlus_VSPackageCmdSet = new Guid(guidNuGetTemplatesPlus_VSPackageCmdSetString);
    };
}