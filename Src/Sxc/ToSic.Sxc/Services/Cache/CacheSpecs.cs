using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Caching;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using static ToSic.Sxc.Services.Cache.CacheServiceConstants;

namespace ToSic.Sxc.Services.Cache;

internal class CacheSpecs(ICodeApiService codeApiSvc, LazySvc<IAppStates> appStates, CacheKeySpecs key, IPolicyMaker policyMaker): ICacheSpecs
{
    #region Keys

    /// <summary>
    /// Generate the final cache key.
    /// This services will always add a prefix to the key, to avoid conflicts with other cache keys.
    /// 
    /// This happens automatically, the Key method is only needed if you want to see a key manually, mainly for debugging purposes.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string Key => key.Key;

    #endregion

    public IPolicyMaker PolicyMaker => policyMaker;

    private ICacheSpecs Next(IPolicyMaker newPm) => new CacheSpecs(codeApiSvc, appStates, key, newPm);
    private ICacheSpecs Next(string varyBy) => new CacheSpecs(codeApiSvc, appStates, key with { VaryBy = $"{Sep}{key.VaryBy}{varyBy}" }, policyMaker);

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration) 
        => Next(policyMaker.SetAbsoluteExpiration(absoluteExpiration));

    public ICacheSpecs SetSlidingExpiration(TimeSpan slidingExpiration)
        => Next(policyMaker.SetSlidingExpiration(slidingExpiration));

    public ICacheSpecs WatchFile(string filePath)
        => Next(policyMaker.AddFiles([filePath]));

    public ICacheSpecs WatchFiles(IEnumerable<string> filePaths)
        => Next(policyMaker.AddFiles([..filePaths]));

    public ICacheSpecs WatchFolder(string folderPath, bool watchSubfolders = false)
        => Next(policyMaker.AddFolders(new Dictionary<string, bool> { { folderPath, watchSubfolders } }));

    public ICacheSpecs WatchFolders(IDictionary<string, bool> folderPaths)
        => Next(policyMaker.AddFolders(folderPaths));

    public ICacheSpecs WatchCacheKeys(IEnumerable<string> cacheKeys)
        => Next(policyMaker.AddCacheKeys(cacheKeys));

    public ICacheSpecs VaryByUser(ICmsUser user = default)
        => Next($"{Sep}VaryByUser:{(user?.Id ?? codeApiSvc?.CmsContext?.User?.Id)?.ToString() ?? "unknown"}");

    public ICacheSpecs WatchApp()
#pragma warning disable CS0618 // Type or member is obsolete
        => Next(policyMaker.AddAppStates([appStates.Value.GetCacheState(codeApiSvc.App.AppId) as IAppStateChanges]));
#pragma warning restore CS0618 // Type or member is obsolete

    //public ICacheTweak AddAppStates(List<IAppStateChanges> appStates)
    //    => new CacheTweak(policyMaker.AddAppStates(appStates), keyAdditions);

    //public ICacheTweak ConnectFeaturesService(IEavFeaturesService featuresService)
    //    => new CacheTweak(policyMaker.ConnectFeaturesService(featuresService), keyAdditions);

    //public ICacheTweak AddUpdateCallback(CacheEntryUpdateCallback updateCallback)
    //    => new CacheTweak(policyMaker.AddUpdateCallback(updateCallback), keyAdditions);
}