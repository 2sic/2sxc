using ToSic.Eav.Caching;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyCacheManager(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".AssCMn", connect: [memoryCacheService])
{
    private const string GlobalCacheRoot = "Sxc-AssemblyCache.App.";


    #region Static Calls for AppCode - to use before requiring DI
    public (AssemblyResult AssemblyResult, string cacheKey) TryGetAppCode(HotBuildSpec spec)
    {
        var cacheKey = KeyAppCode(spec);
        return (Get(cacheKey), cacheKey);
    }
    private static string KeyAppCode(HotBuildSpec spec) => $"{GlobalCacheRoot}app:{spec.AppId}.edition:{spec.Edition}.AppCode";
    #endregion

    #region Static Calls for Dependecies - to use before requiring DI
    public (List<AssemblyResult> assemblyResults, string cacheKey) TryGetDependencies(HotBuildSpec spec)
    {
        var cacheKey = KeyDependency(spec);
        return (memoryCacheService.Get<List<AssemblyResult>>(cacheKey), cacheKey);
    }
    private static string KeyDependency(HotBuildSpec spec) => $"{GlobalCacheRoot}app:{spec.AppId}.edition:{spec.Edition}.dep:{DependenciesLoader.DependenciesFolder}";
    #endregion

    #region Static Calls Only - for use before the object is created using DI

    internal static string KeyTemplate(string templateFullPath) => $"{GlobalCacheRoot}view:{templateFullPath.ToLowerInvariant()}";

    private AssemblyResult Get(string key) => memoryCacheService.Get<AssemblyResult>(key);

    public AssemblyResult TryGetTemplate(string templateFullPath) => Get(KeyTemplate(templateFullPath));

    #endregion

    public string Add(string cacheKey, object data, int slidingDuration, IList<string> filePaths = null, IDictionary<string, bool> folderPaths = null, IEnumerable<ICanBeCacheDependency> dependencies = default)
    {
        var l = Log.Fn<string>($"{nameof(cacheKey)}: {cacheKey}; {nameof(slidingDuration)}: {slidingDuration}", timer: true);

        // Never store 0, that's like never-expire
        if (slidingDuration <= 0)
            return l.ReturnAsError("slidingDuration must be greater than 0");

        // Try to add to cache - use try-catch to avoid exceptions
        try
        {
            memoryCacheService.SetNew(cacheKey, data, p => p
                .SetSlidingExpiration(slidingDuration)
                .WatchFiles(filePaths)
                .WatchFolders(folderPaths)
                .WatchNotifyKeys(dependencies)
            );
            // Ported 2024-10-22 - remove old code ca. 2024-12 #MemoryCacheApiCleanUp
            //var cacheSpecs = memoryCacheService.NewPolicyMaker()
            //    .SetSlidingExpiration(new(0, 0, slidingDuration))
            //    .WatchFiles(filePaths)
            //    .WatchFolders(folderPaths)
            //    .WatchNotifyKeys(dependencies);
            //memoryCacheService.SetNew(cacheKey, data, cacheSpecs);

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