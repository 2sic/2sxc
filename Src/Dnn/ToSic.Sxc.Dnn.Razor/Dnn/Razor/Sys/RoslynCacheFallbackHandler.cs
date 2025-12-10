using System.CodeDom.Compiler;
using System.Reflection;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Handles reuse of existing assemblies when the compiler cannot overwrite locked files.
/// </summary>
public class RoslynCacheFallbackHandler(
    IAssemblyDiskCacheService diskCacheService,
    TemplateCacheService cacheService,
    AssemblyDiskCache diskCache)
    : ServiceBase("Dnn.RzCacheFb", connect: [diskCacheService, cacheService, diskCache])
{
    public AssemblyResult TryUseExisting(
        CodeFileInfo codeFileInfo,
        HotBuildSpec spec,
        string className,
        bool isCshtml,
        string contentHash,
        TemplateCacheService.AppCodeCacheInfo appCodeInfo,
        string outputAssemblyPath,
        List<CompilerError> errors,
        AssemblyResult appCodeAssemblyResult,
        Func<Assembly, AssemblyResult> createResult)
    {
        var l = Log.Fn<AssemblyResult>(timer: true);

        if (errors == null || !IsWriteLockError(errors))
            return l.ReturnNull();

        var cached = diskCacheService.TryLoadFromCache(
            spec,
            codeFileInfo.RelativePath,
            contentHash,
            appCodeInfo.Hash,
            appCodeInfo.AssemblyResult,
            codeFileInfo);

        if (cached != null)
        {
            l.A($"Compiler couldn't write to '{outputAssemblyPath}' (locked). Using cached assembly (hash match).");
            cacheService.AddToCache(codeFileInfo, spec, cached, contentHash, appCodeInfo.Hash, appCodeInfo.AssemblyResult);
            return l.ReturnAsOk(cached);
        }

        if (!outputAssemblyPath.HasValue() || !File.Exists(outputAssemblyPath) || !diskCacheService.IsEnabled())
            return l.ReturnNull();

        l.A($"Attempting to load locked assembly directly from '{outputAssemblyPath}'.");

        Assembly loaded;
        try
        {
            loaded = diskCache.LoadWithRetry(
                outputAssemblyPath,
                loadAssembly: path => Assembly.Load(File.ReadAllBytes(path)));
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            l.E($"Failed to load locked assembly '{outputAssemblyPath}' via retry helper: {ex.Message}");
            return l.ReturnNull();
        }

        //// ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //if (loaded == null)
        //    return l.ReturnNull();

        var result = createResult(loaded);
        cacheService.AddToCache(codeFileInfo, spec, result, contentHash, appCodeInfo.Hash, appCodeInfo.AssemblyResult);
        l.A($"Loaded locked assembly from '{outputAssemblyPath}' after write error.");
        return l.ReturnAsOk(result);
    }

    private static bool IsWriteLockError(IEnumerable<CompilerError> errors)
        => errors.Where(e => !e.IsWarning).Any(error
            => string.Equals(error.ErrorNumber, "CS0016", StringComparison.OrdinalIgnoreCase)
            || error.ErrorText?.IndexOf("because it is being used by another process", StringComparison.OrdinalIgnoreCase) >= 0
            || (error.ErrorText?.IndexOf("Cannot open", StringComparison.OrdinalIgnoreCase) >= 0
                && error.ErrorText?.IndexOf("for writing", StringComparison.OrdinalIgnoreCase) >= 0));
}
