using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Sys.Caching.Policies;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Converts cache configuration to cache-policy maker.
/// </summary>
internal class CacheConfigToPolicyMaker(CacheContextTools CacheContextTools)
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
        return policyMaker.WatchNotifyKeys([CacheContextTools.AppReader.GetCache()]);
    }

    private IPolicyMaker ApplyWatchAppFolder(IPolicyMaker policyMaker, CacheWriteConfig keyConfig)
    {
        if (!keyConfig.WatchAppFolder)
            return policyMaker;
        var appPaths = CacheContextTools.AppPathsLazy.New().Get(CacheContextTools.AppReader, CacheContextTools.Site);
        return policyMaker.WatchFolders(new Dictionary<string, bool>
        {
            [appPaths.PhysicalPath] = keyConfig.WatchAppSubfolders
        });
    }

    internal IPolicyMaker ApplySlidingExpiration(IPolicyMaker policyMaker, CacheKeyConfig keyConfig)
    {
        var slidingAny = keyConfig.SecondsFor(CacheContextTools.UserElevation);
        return slidingAny > 0
            ? policyMaker.SetSlidingExpiration(slidingAny)
            : policyMaker;
    }
}
