using System.IO;

namespace ToSic.Sxc.Dnn.Compile.AppDomain;

[PrivateApi]
internal class CSharpAssemblyLoader : AssemblyLoader
{
    internal string GetLanguageVersions()
    {
        var assemblyPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "roslyn", "Microsoft.CodeAnalysis.CSharp.dll");
        if (!File.Exists(assemblyPath)) return string.Empty;
        var enumType = LoadAssemblyAndGetTypeInfo(assemblyPath, "Microsoft.CodeAnalysis.CSharp.LanguageVersion");
        if (enumType == null || enumType.IsEnum == false) return string.Empty;
        return string.Join(",", (int[])Enum.GetValues(enumType));
    }
}