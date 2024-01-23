using System.Diagnostics;
using System.IO;
using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ThisAppCompiler() : ServiceBase("Sxc.MyApCd")
{
    public const string CsFiles = ".cs";
    public const bool UseSubfolders = true;
    public const string ThisAppDll = "ThisApp.dll";

    protected internal abstract AssemblyResult GetThisApp(string relativePath, HotBuildSpec spec);

    protected string[] GetSourceFiles(string fullPath)
    {
        var l = Log.Fn<string[]>(timer: true);

        if (!Directory.Exists(fullPath))
            return l.ReturnAsOk([]);

        //var sourceFiles = GetSourceFilesInFolder(Path.Combine(fullPath, HotBuildEnum.Code.ToString()))
        //    .Concat(GetSourceFilesInFolder(Path.Combine(fullPath, HotBuildEnum.Data.ToString()))).ToArray();

        // Build the ThisApp folder with subfolders
        var sourceFiles = GetSourceFilesInFolder(fullPath);

        // Log all files
        foreach (var sourceFile in sourceFiles) l.A(sourceFile);

        return l.ReturnAsOk(sourceFiles);
    }

    private static string[] GetSourceFilesInFolder(string fullPath) => Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    protected string GetAppCodeDllName(string folderPath, HotBuildSpec spec)
    {
        var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'; {spec}", timer: true);
        string randomNameWithoutExtension;
        do
        {
            var app = $"App-{spec.AppId:00000}";
            var edition = spec.Edition.HasValue() ? $".{spec.Edition}" : "";
            randomNameWithoutExtension = $"{app}-ThisApp{edition}-{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
        }
        while (File.Exists(Path.Combine(folderPath, $"{randomNameWithoutExtension}.dll")));
        return l.ReturnAsOk(randomNameWithoutExtension);
    }


    /// <summary>
    /// Normalize full file or folder path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    protected static string NormalizeFullPath(string fullPath) => new FileInfo(fullPath).FullName;


    protected void LogAllTypes(Assembly assembly)
    {
        var l = Log.Fn<bool>(assembly?.FullName);

        var list = AssemblyAnalyzer.TypeInformation(assembly);
        foreach (var item in list) l.A(item);

        l.Done();
    }
}