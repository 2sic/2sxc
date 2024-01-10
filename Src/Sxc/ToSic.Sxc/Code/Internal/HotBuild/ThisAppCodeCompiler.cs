using System.IO;
using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class ThisAppCodeCompiler() : ServiceBase("Sxc.MyApCd")
{
    public const string CsFiles = ".cs";
    public const bool UseSubfolders = false;
    public const string ThisAppCodeDll = "ThisApp.Code.dll";

    protected internal abstract AssemblyResult GetAppCode(string relativePath, HotBuildSpec spec);

    protected (string[] SourceFiles, AssemblyResult ErrorResult) GetSourceFilesOrError(string fullPath)
    {
        var l = Log.Fn<(string[], AssemblyResult)>(timer: true);

        var sourceFiles = Directory.GetFiles(fullPath, $"*{CsFiles}", UseSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        // Log all files
        foreach (var sourceFile in sourceFiles) l.A(sourceFile);

        // Validate are there any C# files
        // TODO: if no files exist, it shouldn't be an error, because it could be that it's just not here yet
        return sourceFiles.Length == 0
            ? l.ReturnAsError((sourceFiles, new AssemblyResult(errorMessages: $"Error: given path '{fullPath}' doesn't contain any {CsFiles} files"))) :
            l.ReturnAsOk((sourceFiles, null));
    }

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    protected virtual string GetAppCodeDllName(string folderPath, HotBuildSpec spec)
    {
        var l = Log.Fn<string>($"{nameof(folderPath)}: '{folderPath}'; {nameof(spec.AppId)}: {spec.AppId}; ; {nameof(spec.Edition)}: '{spec.Edition}'", timer: true);
        string randomNameWithoutExtension;
        do
        {
            var appIdWithEdition = spec.Edition.HasValue() ? $"{spec.AppId:00000}-{spec.Edition}" : $"{spec.AppId:00000}";
            randomNameWithoutExtension = $"ThisApp.Code-{appIdWithEdition}-{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}";
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