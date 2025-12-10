using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Coordinates Razor compilation by delegating heavy work to <see cref="RoslynCompilationRunner"/>.
/// Ensures caching/locking concerns stay isolated.
/// </summary>
public class RoslynBuildManager(
    TemplateCacheService cacheService,
    RoslynCompilationRunner compilationRunner)
    : ServiceBase("Dnn.RoslynBuildManager", connect: [cacheService, compilationRunner]),
        IRoslynBuildManager
{
    private static readonly NamedLocks CompileAssemblyLocks = new();

    /// <summary>
    /// Manage template compilations, cache the assembly and return the generated type.
    /// </summary>
    public Type GetCompiledType(CodeFileInfo codeFileInfo, HotBuildSpec spec)
        => GetCompiledAssembly(codeFileInfo, null, spec)?.MainType;

    public AssemblyResult GetCompiledAssembly(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};");
        var lockObject = CompileAssemblyLocks.Get(codeFileInfo.FullPath!);
        var cachedResult = cacheService.TryGetFromCache(codeFileInfo, spec);
        var cacheMissLogged = false;

        var (result, generated, message) = new TryLockTryDo(lockObject).Call(
            conditionToGenerate: ShouldGenerate,
            generator: () => compilationRunner.Compile(codeFileInfo, className, spec),
            cacheOrFallback: CacheOrFallback);

        if (!generated)
            LogCacheFallback(result, message, codeFileInfo, l);

        return l.Return(result, message);

        AssemblyResult CacheOrFallback()
            => cachedResult ?? cacheService.TryGetFromCache(codeFileInfo, spec);

        bool ShouldGenerate()
        {
            if (cachedResult?.MainType != null)
                return false;

            cachedResult = cacheService.TryGetFromCache(codeFileInfo, spec);
            if (cachedResult?.MainType != null)
                return false;

            if (!cacheMissLogged)
            {
                l.A("Cache miss - will compile template");
                cacheMissLogged = true;
            }

            return true;
        }
    }

    private static void LogCacheFallback(AssemblyResult result, string message, CodeFileInfo codeFileInfo, ILog l)
    {
        l.A("Object retrieved from cache during lock wait");
        l.A($"{nameof(result)}: {result}");
        l.A($"{nameof(result.MainType)}: {result?.MainType}");
        l.A($"{nameof(result.HasAssembly)}: {result?.HasAssembly}");
        l.A($"{nameof(message)}: {message}");
        l.A($"{nameof(codeFileInfo)}: {codeFileInfo}");
    }
}
