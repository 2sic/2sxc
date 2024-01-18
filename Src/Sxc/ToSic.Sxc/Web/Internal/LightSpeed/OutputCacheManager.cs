using System.Runtime.Caching;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class OutputCacheManager : ServiceBase
{
    internal OutputCacheManager() : base(SxcLogging.SxcLogName + ".OutputCacheManager")
    { }

    internal const string GlobalCacheRoot = "2sxc.Lightspeed.Module.";

    internal string Id(int moduleId, int pageId, int? userId, string view, string suffix, string currentCulture)
    {
        var id = $"{GlobalCacheRoot}p:{pageId}-m:{moduleId}";   
        if (userId.HasValue) id += $"-u:{userId.Value}";
        if (view != null) id += $"-v:{view}";
        if (suffix != null) id += $"-s:{suffix}";
        if (currentCulture != null) id += $"-c:{currentCulture}";
        return id;
    }

    public string Add(string cacheKey, OutputCacheItem data, int duration, IEavFeaturesService features,
        List<IAppStateChanges> appStates, IList<string> appPaths = null, CacheEntryUpdateCallback updateCallback = null)
    {
        var l = Log.Fn<string>($"key: {cacheKey}", timer: true);
        try
        {
            // Never store 0, that's like never-expire
            if (duration == 0) duration = 1;
            var expiration = new TimeSpan(0, 0, duration);
            var policy = new CacheItemPolicy { SlidingExpiration = expiration };

            // flush cache when any feature is changed
            Log.Do(message: "changeMonitors add FeaturesResetMonitor", timer: true, action: () => 
                policy.ChangeMonitors.Add(new FeaturesResetMonitor(features)));

            // get new instance of ChangeMonitor and insert it to the cache item
            if (appStates.Any())
                foreach (var appState in appStates)
                    Log.Do(message: "changeMonitors add AppResetMonitor", timer: true, action: () => 
                        policy.ChangeMonitors.Add(new AppResetMonitor(appState)));

            if (appPaths is { Count: > 0 })
                Log.Do(message: "changeMonitors add FolderChangeMonitor", timer: true, action: () =>
                    policy.ChangeMonitors.Add(new FolderChangeMonitor(appPaths)));

            if (updateCallback != null)
                policy.UpdateCallback = updateCallback;

            Log.Do(message: $"cache set cacheKey:{cacheKey}", timer: true, action: () => 
                Cache.Set(new(cacheKey, data), policy));

            return l.ReturnAsOk(cacheKey);
        }
        catch
        {
            /* ignore for now */
        }
        return l.ReturnAsError("error");
    }

    public OutputCacheItem Get(string key) => Cache[key] as OutputCacheItem;

    private static MemoryCache Cache => MemoryCache.Default;
}