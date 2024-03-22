using System.Runtime.Caching;
using ToSic.Eav.Caching;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyCacheManager(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".AssCMn")
{
    private const string GlobalCacheRoot = "2sxc.AssemblyCache.Module.";

    
    #region Static Calls for AppCode - to use before requiring DI
    public static (AssemblyResult Result, string cacheKey) TryGetAppCode(HotBuildSpec spec)
    {
        var cacheKey = KeyAppCode(spec);
        return (Get(cacheKey), cacheKey);
    }
    private static string KeyAppCode(HotBuildSpec spec) => $"{GlobalCacheRoot}a:{spec.AppId}.e:{spec.Edition}.AppCode";
    #endregion

    #region Static Calls for Dependecies - to use before requiring DI
    public static (List<AssemblyResult> assemblyResults, string cacheKey) TryGetDependencies(HotBuildSpec spec)
    {
        var cacheKey = KeyDependency(spec);
        return (MemoryCacheService.Get(cacheKey) as List<AssemblyResult>, cacheKey);
    }
    private static string KeyDependency(HotBuildSpec spec) => $"{GlobalCacheRoot}a:{spec.AppId}.e:{spec.Edition}.d:{DependenciesLoader.DependenciesFolder}";
    #endregion

    #region Static Calls Only - for use before the object is created using DI

    internal static string KeyTemplate(string templateFullPath) => $"{GlobalCacheRoot}v:{templateFullPath.ToLowerInvariant()}";

    private static AssemblyResult Get(string key) => MemoryCacheService.Get(key) as AssemblyResult;

    public static AssemblyResult TryGetTemplate(string templateFullPath) => Get(KeyTemplate(templateFullPath));

    #endregion

    public string Add(string cacheKey, object data, int duration = 3600, IList<ChangeMonitor> changeMonitor = null, CacheEntryUpdateCallback updateCallback = null)
    {
        var l = Log.Fn<string>($"{nameof(cacheKey)}: {cacheKey}; {nameof(duration)}: {duration}", timer: true);

        // Never store 0, that's like never-expire
        if (duration == 0) duration = 1;
        var expiration = new TimeSpan(0, 0, duration);
        var policy = new CacheItemPolicy { SlidingExpiration = expiration };

        // Try set app change folder monitor
        if (changeMonitor?.Any() == true)
            try
            {
                l.Do(message: $"add {nameof(changeMonitor)}", timer: true, action: () =>
                {
                    foreach (var changeMon in changeMonitor)
                        policy.ChangeMonitors.Add(changeMon);
                });
            }
            catch (Exception ex)
            {
                l.E("Error during set app folder ChangeMonitor");
                l.Ex(ex);
                /* ignore for now */
                return l.ReturnAsError("error");
            }

        // Register Callback - usually to remove something from another cache
        if (updateCallback != null)
            policy.UpdateCallback = updateCallback;

        // Try to add to cache
        try
        {
            l.Do(message: $"cache set cacheKey:{cacheKey}", timer: true,
                action: () => memoryCacheService.Set(new(cacheKey, data), policy));

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