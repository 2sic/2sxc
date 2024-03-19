using System.Runtime.Caching;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Caching;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

internal class OutputCacheManager(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".OutputCacheManager")
{
    internal string Id(int moduleId, int pageId, int? userId, string view, string suffix, string currentCulture)
        => MemoryCacheService.OutputCacheManagerCacheKey(moduleId, pageId, userId, view, suffix, currentCulture);

    public string Add(string cacheKey, OutputCacheItem data, int duration, IEavFeaturesService features,
        List<IAppStateChanges> appStates, IList<string> appPaths = null, CacheEntryUpdateCallback updateCallback = null)
        => memoryCacheService.Add(cacheKey, data, duration, features, appStates, appPaths, updateCallback);

    public OutputCacheItem Get(string key) => memoryCacheService.Get<OutputCacheItem>(key);
}