using System;
using System.IO;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Internal;

[PrivateApi]
internal class AppCodeCompilerNetCore(LazySvc<IServerPaths> serverPaths, Generator<Compiler> compiler, IGlobalConfiguration globalConfiguration, SourceCodeHasher sourceCodeHasher) : AppCodeCompiler(globalConfiguration, sourceCodeHasher, connect: [serverPaths, compiler, sourceCodeHasher])
{
    private readonly TryLockTryDo _lockAppCodeAssemblyProvider = new();

    protected internal override AssemblyResult GetAppCode(string virtualPath, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {spec}", timer: true);

        try
        {
            var sourceRootPath = NormalizeFullPath(serverPaths.Value.FullContentPath(virtualPath.Backslash()));
            var sourceFiles = GetSourceFiles(sourceRootPath);
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec, sourceRootPath);
            var dllName = Path.GetFileName(assemblyPath);

            var result = _lockAppCodeAssemblyProvider.Call(
                conditionToGenerate: () => !File.Exists(assemblyPath) || new FileInfo(assemblyPath).Length == 0 || IsFileLocked(assemblyPath),
                generator: () => compiler.New().GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath, symbolsPath, dllName, spec),
                cacheOrFallback: () => new AssemblyResult(assembly: new SimpleUnloadableAssemblyLoadContext().LoadFromAssemblyPath(assemblyPath))
            );

            var assemblyResult = result.Result;

            var dicInfos = new Dictionary<string, string>
            {
                ["DllName"] = dllName,
                ["Files"] = sourceFiles.Length.ToString(),
                ["Errors"] = assemblyResult.ErrorMessages?.Length.ToString(),
                ["Assembly"] = assemblyResult.Assembly?.FullName ?? "null",
                ["AssemblyPath"] = assemblyPath,
                ["SymbolsPath"] = symbolsPath,
            };

            if (!assemblyResult.ErrorMessages.IsEmpty())
                return l.ReturnAsError(new(errorMessages: assemblyResult.ErrorMessages, infos: dicInfos), assemblyResult.ErrorMessages);

            // Compile ok
            LogAllTypes(assemblyResult.Assembly);
            return l.ReturnAsOk(new(assembly: assemblyResult.Assembly, assemblyLocations: [symbolsPath, assemblyPath], infos: dicInfos));

        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{AppCodeDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new(errorMessages: errorMessage), "error");
        }
    }

    private static bool IsFileLocked(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);

            // Check if the file is read-only
            if (fileInfo.IsReadOnly)
                return true;

            // Try to open the file with FileShare.None to check if it is locked
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return !stream.CanRead;
        }
        catch (IOException)
        {
            // If an IOException is thrown, the file is locked
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // If an UnauthorizedAccessException is thrown, the file is locked
            return true;
        }
        catch (Exception)
        {
            // Handle any other exceptions that might occur
            return true;
        }
    }
}