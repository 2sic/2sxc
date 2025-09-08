using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sys.Configuration;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Code.Sys;

[PrivateApi]
internal class AppCodeCompilerNetCore(
    LazySvc<IServerPaths> serverPaths,
    Generator<Compiler> compiler,
    IGlobalConfiguration globalConfiguration,
    SourceCodeHasher sourceCodeHasher)
    : AppCodeCompiler(globalConfiguration, sourceCodeHasher, connect: [serverPaths, compiler, sourceCodeHasher])
{

    public override AssemblyResult GetAppCode(string virtualPath, HotBuildSpecWithSharedSuffix spec)
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

            var result = LockAppCodeAssemblyProvider.Call(
                conditionToGenerate: () => ShouldGenerate(assemblyPath),
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

            // Success
            if (assemblyResult.ErrorMessages.IsEmpty())
            {
                LogAllTypes(assemblyResult.Assembly);
                return l.ReturnAsOk(new(assemblyResult.Assembly)
                {
                    AssemblyLocations = [symbolsPath, assemblyPath],
                    Infos = dicInfos,
                });
            }

            // Errors and warnings
            return l.ReturnAsError(new()
            {
                ErrorMessages = assemblyResult.ErrorMessages,
                Infos = dicInfos,
            }, assemblyResult.ErrorMessages);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{AppCodeDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new() { ErrorMessages = errorMessage, });
        }
    }
}