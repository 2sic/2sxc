using System.IO;
using System.Reflection;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class AppCodeCompiler(IGlobalConfiguration globalConfiguration, SourceCodeHasher sourceCodeHasher, object[] connect = default) : ServiceBase("Sxc.MyApCd", connect: connect)
{
    protected const string AppCodeDll = "AppCode.dll";
    private const string CsFiles = ".cs";
    private const bool UseSubfolders = true;

    protected internal abstract AssemblyResult GetAppCode(string relativePath, HotBuildSpecWithSharedSuffix spec);

    protected string[] GetSourceFiles(string fullPath)
    {
        var l = Log.Fn<string[]>(timer: true);

        if (!Directory.Exists(fullPath))
            return l.ReturnAsOk([]);

        //var sourceFiles = GetSourceFilesInFolder(Path.Combine(fullPath, HotBuildEnum.Code.ToString()))
        //    .Concat(GetSourceFilesInFolder(Path.Combine(fullPath, HotBuildEnum.Data.ToString()))).ToArray();

        // Build the AppCode folder with subfolders
        var sourceFiles = sourceCodeHasher.GetSourceFilesInFolder(fullPath);

        // Log all files
        foreach (var sourceFile in sourceFiles) l.A(sourceFile);

        return l.ReturnAsOk(sourceFiles);
    }

    /// <summary>
    /// Generates a name with hash for a dll file.
    /// </summary>
    /// <returns>The generated random name.</returns>
    private string GetAppCodeDllName(string sourceRootPath, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<string>($"{nameof(sourceRootPath)}: '{sourceRootPath}'; {spec}", timer: true);
        var assemblyName = $"App-{spec.AppId:00000}-AppCode{OptionalSuffix(spec)}";
        return l.ReturnAsOk(HashInNameWithoutExtension(sourceRootPath, assemblyName));
    }

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    private string GetDependencyDllName(string folderPath, HotBuildSpecWithSharedSuffix spec, string dependency)
    {
        var l = Log.Fn<string>($"{nameof(dependency)}: '{dependency}'; {nameof(folderPath)}: '{folderPath}'; {spec}", timer: true);
        var dependencyFileName = Path.GetFileNameWithoutExtension(dependency);
        var assemblyName = $"App-{spec.AppId:00000}-Dependency{OptionalSuffix(spec)}-{dependencyFileName}";
        return l.ReturnAsOk(RandomNameWithoutExtension(folderPath, assemblyName));
    }

    private static string OptionalSuffix(HotBuildSpecWithSharedSuffix spec)
    {
        var optionalEditionSuffix = spec.Edition.HasValue() ? $".{spec.Edition}" : "";
        var optionalSharedSuffix = spec.SharedSuffix.HasValue() ? $".{spec.SharedSuffix}" : "";
        return $"{optionalEditionSuffix}{optionalSharedSuffix}";
    }

    private string HashInNameWithoutExtension(string folderPath, string assemblyName)
        => $"{assemblyName}-{sourceCodeHasher.GetHashString(folderPath)}";

    private static string RandomNameWithoutExtension(string folderPath, string assemblyName)
    {
        string randomNameWithoutExtension;
        do
        {
            var randomSuffix = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            randomNameWithoutExtension = $"{assemblyName}-{randomSuffix}";
        }
        while (File.Exists(Path.Combine(folderPath, $"{randomNameWithoutExtension}.dll")));

        return randomNameWithoutExtension;
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

    protected (string SymbolsPath, string AssemblyPath) GetAssemblyLocations(HotBuildSpecWithSharedSuffix spec, string sourceRootPath)
    {
        var l = Log.Fn<(string, string)>($"{spec}");
        l.A($"TempAssemblyFolderPath: '{globalConfiguration.TempAssemblyFolder}'");

        // need name 
        var assemblyName = GetAppCodeDllName(sourceRootPath, spec);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(globalConfiguration.TempAssemblyFolder, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(globalConfiguration.TempAssemblyFolder, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = (symbolsFilePath, assemblyFilePath);
        return l.ReturnAsOk(assemblyLocations);
    }

    protected internal string GetDependencyAssemblyLocations(string dependency, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<string>($"{spec}");
        l.A($"TempAssemblyFolderPath: '{globalConfiguration.TempAssemblyFolder}'");

        // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
        var assemblyName = GetDependencyDllName(globalConfiguration.TempAssemblyFolder, spec, dependency);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(globalConfiguration.TempAssemblyFolder, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");

        return l.ReturnAsOk(assemblyFilePath);
    }
}