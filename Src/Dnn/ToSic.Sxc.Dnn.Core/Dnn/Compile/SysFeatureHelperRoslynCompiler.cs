using System.IO;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
internal class RoslynCompilerCapability
{
    internal static bool CheckCsharpLangVersion(int version) => CsharpLangVersions.Contains(value: version);


    private static int[] CsharpLangVersions => _csharpLangVersions ??= GetCsharpLangVersions();
    private static volatile int[] _csharpLangVersions;

    /// <summary>
    /// Goal is that it can tell if the newer CodeDom library has been installed or not
    /// used to build a config in the App, so the app can warn if a feature is missing
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is optimized version, it is just checking for "/bin/roslyn/Microsoft.CodeAnalysis.CSharp.dll" file.
    /// Older version where checking for Microsoft.CodeAnalysis.CSharp.LanguageVersion enum, but has performance problems
    /// while using reflection on tmp loaded assembly in new application domain that can be unloaded,
    /// also used a double-check locking pattern to ensure thread safety and performance.
    /// </remarks>
    private static int[] GetCsharpLangVersions() 
        => (!File.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "roslyn", "Microsoft.CodeAnalysis.CSharp.dll")))
            ? []
            : [ 0, 1, 2, 3, 4, 5, 6, 7,
                701, // 0x000002BD
                702, // 0x000002BE
                703, // 0x000002BF
                800, // 0x00000320
                2147483645, // LatestMajor - 0x7FFFFFFD
                2147483646, // Preview - 0x7FFFFFFE
                2147483647, // Latest - 0x7FFFFFFF
              ];
}