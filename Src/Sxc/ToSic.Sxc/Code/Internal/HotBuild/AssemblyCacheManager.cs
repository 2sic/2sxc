using ToSic.Eav.Caching;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyCacheManager(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".AssCMn", connect: [memoryCacheService])
{
    private const string GlobalCacheRoot = "2sxc.AssemblyCache.Module.";


    #region Static Calls for AppCode - to use before requiring DI
    public (AssemblyResult AssemblyResult, string cacheKey) TryGetAppCode(HotBuildSpec spec)
    {
        var cacheKey = KeyAppCode(spec);
        return (Get(cacheKey), cacheKey);
    }
    private static string KeyAppCode(HotBuildSpec spec) => $"{GlobalCacheRoot}a:{spec.AppId}.e:{spec.Edition}.AppCode";
    #endregion

    #region Static Calls for Dependecies - to use before requiring DI
    public (List<AssemblyResult> assemblyResults, string cacheKey) TryGetDependencies(HotBuildSpec spec)
    {
        var cacheKey = KeyDependency(spec);
        return (memoryCacheService.Get<List<AssemblyResult>>(cacheKey), cacheKey);
    }
    private static string KeyDependency(HotBuildSpec spec) => $"{GlobalCacheRoot}a:{spec.AppId}.e:{spec.Edition}.d:{DependenciesLoader.DependenciesFolder}";
    #endregion

    #region Static Calls Only - for use before the object is created using DI

    internal static string KeyTemplate(string templateFullPath) => $"{GlobalCacheRoot}v:{templateFullPath.ToLowerInvariant()}";

    private AssemblyResult Get(string key) => memoryCacheService.Get<AssemblyResult>(key);

    public AssemblyResult TryGetTemplate(string templateFullPath) => Get(KeyTemplate(templateFullPath));

    #endregion

    public string Add(string cacheKey, object data, int slidingDuration = CacheConstants.Duration1Hour, IList<string> filePaths = null, IDictionary<string, bool> folderPaths = null, IEnumerable<string> keys = null)
    {
        var l = Log.Fn<string>($"{nameof(cacheKey)}: {cacheKey}; {nameof(slidingDuration)}: {slidingDuration}", timer: true);

        // Never store 0, that's like never-expire
        if (slidingDuration == 0) slidingDuration = 1;
        var expiration = new TimeSpan(0, 0, slidingDuration);

        // Try to add to cache
        try
        {
            l.A($"cache set cacheKey:{cacheKey}");
            memoryCacheService.Set(cacheKey, data, slidingExpiration: expiration, filePaths: filePaths, folderPaths: folderPaths, cacheKeys: keys);

            return l.ReturnAsOk(cacheKey);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            /* ignore for now */
            return l.ReturnAsError("error");
        }
    }
}