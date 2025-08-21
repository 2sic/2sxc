using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Sys.Caching.Policies;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Converts cache configuration to cache-policy maker.
/// </summary>
internal class CacheConfigToPolicyMaker(CacheContextTools cacheContextTools)
{
    internal IPolicyMaker ReplayConfig(IPolicyMaker policyMaker, CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        // Only set either the absolute or sliding expiration, never both.
        policyMaker = writeConfig.AbsoluteExpiration != default
            ? policyMaker.SetAbsoluteExpiration(writeConfig.AbsoluteExpiration)
            : ApplySlidingExpiration(policyMaker, keyConfig);

        policyMaker = ApplyWatchAppData(policyMaker, writeConfig);
        policyMaker = ApplyWatchAppFolder(policyMaker, writeConfig);

        return policyMaker;
    }



    private IPolicyMaker ApplyWatchAppData(IPolicyMaker policyMaker, CacheWriteConfig keyConfig)
    {
        return policyMaker.WatchNotifyKeys([cacheContextTools.AppReader.GetCache()]);
    }

    private IPolicyMaker ApplyWatchAppFolder(IPolicyMaker policyMaker, CacheWriteConfig keyConfig)
    {
        if (!keyConfig.WatchAppFolder)
            return policyMaker;
        var appPaths = cacheContextTools.AppPathsLazy.New().Get(cacheContextTools.AppReader, cacheContextTools.Site);
        return policyMaker.WatchFolders(new Dictionary<string, bool>
        {
            [appPaths.PhysicalPath] = keyConfig.WatchAppSubfolders
        });
    }

    private IPolicyMaker ApplySlidingExpiration(IPolicyMaker policyMaker, CacheKeyConfig keyConfig)
    {
        var slidingAny = keyConfig.SecondsFor(cacheContextTools.UserElevation);
        return slidingAny > 0
            ? policyMaker.SetSlidingExpiration(slidingAny)
            : policyMaker;
    }
}
