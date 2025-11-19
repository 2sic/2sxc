using System.Reflection;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Configuration;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Code.Sys.HotBuild;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class AppCodeCompiler(
    IGlobalConfiguration globalConfiguration,
    SourceCodeHasher sourceCodeHasher,
    object[]? connect = default)
    : ServiceBase("Sxc.MyApCd", connect: connect)
{
    protected const string AppCodeDll = "AppCode.dll";

    public abstract AssemblyResult GetAppCode(string relativePath, HotBuildSpecWithSharedSuffix spec);

    protected string[] GetSourceFiles(string fullPath)
    {
        var l = Log.Fn<string[]>(timer: true);

        if (!Directory.Exists(fullPath))
            return l.ReturnAsOk([]);

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
        var appCodeHash = sourceCodeHasher.GetHashString(sourceRootPath);
        var appCodeHashShort = appCodeHash.Length > 6
            ? appCodeHash.Substring(0, 6)
            : appCodeHash;
        var assemblyName = $"AppCode-{appCodeHashShort}";
        return l.ReturnAsOk(assemblyName);
    }

    /// <summary>
    /// Generates a random name for a dll file and ensures it does not already exist in the "2sxc.bin" folder.
    /// </summary>
    /// <returns>The generated random name.</returns>
    private string GetDependencyDllName(string folderPath, HotBuildSpecWithSharedSuffix spec, string dependency)
    {
        var l = Log.Fn<string>($"{nameof(dependency)}: '{dependency}'; {nameof(folderPath)}: '{folderPath}'; {spec}", timer: true);
        var dependencyFileName = Path.GetFileNameWithoutExtension(dependency);
        var assemblyName = $"dep-{dependencyFileName}";
        return l.ReturnAsOk(RandomNameWithoutExtension(folderPath, assemblyName));
    }

    private static string RandomNameWithoutExtension(string folderPath, string assemblyName)
    {
        Directory.CreateDirectory(folderPath);
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
    protected static string NormalizeFullPath(string fullPath)
        => new FileInfo(fullPath).FullName;


    protected void LogAllTypes(Assembly? assembly)
    {
        var l = Log.Fn<bool>(assembly?.FullName);

        if (assembly == null)
        {
            l.Done("Assembly is null, nothing to log");
            return;
        }

        var list = AssemblyAnalyzer.TypeInformation(assembly);
        foreach (var item in list)
            l.A(item);

        l.Done();
    }

    protected (string SymbolsPath, string AssemblyPath) GetAssemblyLocations(HotBuildSpecWithSharedSuffix spec, string sourceRootPath)
    {
        var l = Log.Fn<(string, string)>($"{spec}");
        var cacheFolder = GetAppAssemblyFolder(spec);
        l.A($"App cache folder: '{cacheFolder}'");

        // need name 
        var assemblyName = GetAppCodeDllName(sourceRootPath, spec);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(cacheFolder, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(cacheFolder, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = (symbolsFilePath, assemblyFilePath);
        return l.ReturnAsOk(assemblyLocations);
    }

    protected internal string GetDependencyAssemblyLocations(string dependency, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<string>($"{spec}");
        var cacheFolder = GetAppAssemblyFolder(spec);
        l.A($"TempAssemblyFolderPath: '{cacheFolder}'");

        // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
        var assemblyName = GetDependencyDllName(cacheFolder, spec, dependency);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(cacheFolder, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");

        return l.ReturnAsOk(assemblyFilePath);
    }

    protected static readonly NamedLocks CompileAssemblyLocks = new();
    protected readonly TryLockTryDo LockAppCodeAssemblyProvider = new();

    protected bool ShouldGenerate(string assemblyPath)
    {
        var l = Log.Fn<bool>(assemblyPath);
        if (!File.Exists(assemblyPath))
            return l.ReturnTrue("should generate, file doesn't exist");

        var fileInfo = new FileInfo(assemblyPath);
        if (fileInfo.Length == 0)
            return l.ReturnTrue("should generate, file empty");

        var isLocked = IsFileLocked(fileInfo, assemblyPath);
        return isLocked
            //? l.ReturnTrue("exists and locked, should generate; - not sure why this would want to regenerate - ask STV")
            ? l.ReturnFalse("exists and locked, should MAYBE NOT generate; - not sure why this would want to regenerate - ask STV")
            : l.ReturnFalse("Just load existing. File exists and is not locked.");
    }

    private bool IsFileLocked(FileInfo fileInfo, string filePath)
    {
        var l = Log.Fn<bool>($"{filePath}");
        try
        {
            // Check if the file is read-only
            if (fileInfo.IsReadOnly)
                return l.ReturnTrue("read only");

            // Try to open the file with FileShare.None to check if it is locked
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var isLocked = !stream.CanRead;
            // Experimental early close, because I'm not sure if automatic disposal happens fast enough
            stream.Close();
            return l.Return(isLocked, $"{nameof(isLocked)}: {isLocked}");
        }
        catch (IOException)
        {
            // If an IOException is thrown, the file is locked
            return l.ReturnTrue(nameof(IOException));
        }
        catch (UnauthorizedAccessException)
        {
            // If an UnauthorizedAccessException is thrown, the file is locked
            return l.ReturnTrue(nameof(UnauthorizedAccessException));
        }
        catch (Exception)
        {
            // Handle any other exceptions that might occur
            return l.ReturnTrue($"{nameof(Exception)} other");
        }
    }

    /// <summary>
    /// Build the per-app cache folder path (app/edition/shared) and ensure it exists.
    /// </summary>
    protected string GetAppAssemblyFolder(HotBuildSpecWithSharedSuffix spec)
    {
        var root = spec.SharedSuffix.HasValue()
            ? Path.Combine(globalConfiguration.TempAssemblyFolder(), spec.SharedSuffix)
            : globalConfiguration.TempAssemblyFolder();
        var edition = NormalizeForFolder(spec.Edition, "root");
        var cacheFolder = Path.Combine(
            root,
            $"{spec.AppId:0000}-{edition}");

        Directory.CreateDirectory(cacheFolder);
        return cacheFolder;
    }

    private static string NormalizeForFolder(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Select(ch => invalid.Contains(ch) ? '-' : ch).ToArray());
        return string.IsNullOrWhiteSpace(cleaned)
            ? fallback
            : cleaned;
    }
}
