using System;
using System.IO;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    internal class CSharpAssemblyLoader : AssemblyLoader
    {
        internal string GetLanguageVersions()
        {
            var assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "roslyn", "Microsoft.CodeAnalysis.CSharp.dll");
            if (!File.Exists(assemblyPath)) return string.Empty;
            var enumType = LoadAssemblyAndGetTypeInfo(assemblyPath, "Microsoft.CodeAnalysis.CSharp.LanguageVersion");
            if (enumType == null || enumType.IsEnum == false) return string.Empty;
            return string.Join(",", (int[])Enum.GetValues(enumType));
        }
    }
}