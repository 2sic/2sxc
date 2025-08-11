using ToSic.Sys.Caching;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

internal class OutputCacheManager(MemoryCacheService memoryCacheService, LazySvc<ISysFeaturesService> featuresDoNotConnect) : ServiceBase(SxcLogName + ".OutputCacheManager", connect: [memoryCacheService])
{
    internal const string GlobalCacheRoot = "Sxc-LightSpeed.Module.";

    internal static string Id(int moduleId, int pageId, int? userId, string? view, string? suffix, string? currentCulture)
    {
        var id = $"{GlobalCacheRoot}p:{pageId}-m:{moduleId}";
        if (userId.HasValue)
            id += $"-u:{userId.Value}";
        if (view != null)
            id += $"-v:{view}";
        if (suffix != null)
            id += $"-s:{suffix}";
        if (currentCulture != null)
            id += $"-c:{currentCulture}";
        return id;
    }

    public string Add(string cacheKey, OutputCacheItem data, int duration, List<string> apps, IList<string>? appPaths)
    {
        var l = Log.Fn<string>($"key: {cacheKey}", timer: true);

        // if we don't have a duration = 0 (which would be never expire), don't even add
        if (duration == 0)
            return l.ReturnAsError("duration 0, will not add");

        try
        {
            // Never store 0, that's like never-expire
            //var expiration = new TimeSpan(0, 0, duration);
            List<string> keys = [.. apps, MemoryCacheService.ExpandDependencyId(featuresDoNotConnect.Value)];
            
            l.A("Keys: " + string.Join(", ", keys));

            // experimental, maybe use as replacement... v17.09+
            var policyMaker = memoryCacheService.NewPolicyMaker()
                .SetSlidingExpiration(duration)   // Never store 0, that's like never-expire
                .WatchCacheKeys(keys);

            if (appPaths?.Any() == true)
                policyMaker = policyMaker
                    .WatchFolders(appPaths.ToDictionary(p => p, _ => true));

            memoryCacheService.Set(cacheKey, data, policyMaker);
            
            return l.ReturnAsOk(cacheKey);
        }
        catch
        {
            /* ignore for now */
        }
        return l.ReturnAsError("error");
    }

    public OutputCacheItem? Get(string key) => memoryCacheService.Get<OutputCacheItem>(key);
}