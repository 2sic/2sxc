using System.Collections.Specialized;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Caching;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.Url;
using static System.StringComparer;

namespace ToSic.Sxc.Services.Cache;

internal class CacheSpecs(ILog parentLog, ICodeApiService codeApiSvc, LazySvc<IAppReaderFactory> appReaders, Generator<IAppPathsMicroSvc> appPathsLazy, CacheKeySpecs key, IPolicyMaker policyMaker): ICacheSpecs
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

    #region Next(...) calls, for functional API

    private ICacheSpecs Next(IPolicyMaker newPm) => new CacheSpecs(parentLog, codeApiSvc, appReaders, appPathsLazy, key, newPm);

    private ICacheSpecs Next(string varyBy, int value) => Next(varyBy, value.ToString());

    private ICacheSpecs Next(string varyBy, string value, bool caseSensitive = false)
    {
        varyBy = "VaryBy" + varyBy;
        varyBy = caseSensitive ? varyBy : varyBy.ToLowerInvariant();
        value = caseSensitive ? value : value.ToLowerInvariant();

        var newDic = new Dictionary<string, string>(key.VaryByDic ?? [], InvariantCultureIgnoreCase)
        {
            [varyBy] = value
        };
        return new CacheSpecs(parentLog, codeApiSvc, appReaders, appPathsLazy, key with { VaryByDic = newDic }, policyMaker);
    }

    #endregion


    #region Time Absolute / Sliding

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration) 
        => Next(policyMaker.SetAbsoluteExpiration(absoluteExpiration));

    public ICacheSpecs SetSlidingExpiration(TimeSpan slidingExpiration)
        => Next(policyMaker.SetSlidingExpiration(slidingExpiration));

    #endregion

    public ICacheSpecs WatchFile(string filePath)
        => Next(policyMaker.WatchFiles([filePath]));

    public ICacheSpecs WatchFiles(IEnumerable<string> filePaths)
        => Next(policyMaker.WatchFiles([..filePaths]));

    public ICacheSpecs WatchFolder(string folderPath, bool watchSubfolders = false)
        => Next(policyMaker.WatchFolders(new Dictionary<string, bool> { { folderPath, watchSubfolders } }));

    public ICacheSpecs WatchFolders(IDictionary<string, bool> folderPaths)
        => Next(policyMaker.WatchFolders(folderPaths));

    public ICacheSpecs WatchCacheKeys(IEnumerable<string> cacheKeys)
        => Next(policyMaker.WatchCacheKeys(cacheKeys));


    public ICacheSpecs WatchAppData(NoParamOrder protector = default)
        => Next(policyMaker.WatchNotifyKeys([AppReader.GetCache()]));

    private IAppReader AppReader => _appReader ??= appReaders.Value.Get(codeApiSvc.App.AppId);
    private IAppReader _appReader;

    public ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = true)
    {
        var appPath = appPathsLazy.New().Get(AppReader, ((ICodeApiServiceInternal)codeApiSvc)?._Block?.Context.Site);
        var mainPath = appPath.PhysicalPath;
        return Next(policyMaker.WatchFolders(new Dictionary<string, bool> { { mainPath, withSubfolders ?? true } }));
    }


    //public ICacheTweak AddAppStates(List<IAppStateChanges> appStates)
    //    => new CacheTweak(policyMaker.AddAppStates(appStates), keyAdditions);

    //public ICacheTweak ConnectFeaturesService(IEavFeaturesService featuresService)
    //    => new CacheTweak(policyMaker.ConnectFeaturesService(featuresService), keyAdditions);

    //public ICacheTweak AddUpdateCallback(CacheEntryUpdateCallback updateCallback)
    //    => new CacheTweak(policyMaker.AddUpdateCallback(updateCallback), keyAdditions);

    #region Vary By Value

    //public ICacheSpecs VaryBy(string value, NoParamOrder protector = default, bool caseSensitive = false)
    //    => Next(value, "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
        => Next(name, value, caseSensitive: caseSensitive);

    #endregion

    #region Vary-By Custom User, QueryString, etc.


    public ICacheSpecs VaryByPageParameter(string name, NoParamOrder protector = default, bool caseSensitive = false)
        => Next($"PageParameter-{name}", codeApiSvc?.CmsContext.Page.Parameters[name] ?? "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false)
        => VaryByParamsInternal("Parameters", parameters, names, caseSensitive: caseSensitive);

    public ICacheSpecs VaryByPageParameters(string names = default, NoParamOrder protector = default,
        bool caseSensitive = false)
        => VaryByParamsInternal("PageParameters",
            codeApiSvc?.CmsContext.Page.Parameters ?? new Parameters { Nvc = [] }, names,
            caseSensitive: caseSensitive);

    private ICacheSpecs VaryByParamsInternal(string varyByName, IParameters parameters, string names, bool caseSensitive = false)
    {
        var all = parameters
            .Filter(names)
            .OrderBy(p => p.Key, comparer: InvariantCultureIgnoreCase)
            .ToList();

        var nvc = all
            .Where(pair => pair.Value.HasValue())
            .Aggregate(new NameValueCollection(),
                (seed, pair) =>
                {
                    seed.Add(pair.Key, pair.Value);
                    return seed;
                });

        var asUrl = nvc.NvcToString();
        return Next(varyByName, asUrl, caseSensitive: caseSensitive);
    }

    #endregion

    #region VaryBy Page, Module, User

    public ICacheSpecs VaryByPage(NoParamOrder protector = default, ICmsPage page = default, int? id = default)
        => Next("Page", id ?? page?.Id ?? codeApiSvc?.CmsContext.Page.Id ?? -1);

    public ICacheSpecs VaryByModule(NoParamOrder protector = default, ICmsModule module = default, int? id = default)
        => Next("Module", id ?? module?.Id ?? codeApiSvc?.CmsContext.Module.Id ?? -1);

    public ICacheSpecs VaryByUser(NoParamOrder protector = default, ICmsUser user = default, int? id = default)
        => Next("User", id ?? user?.Id ?? codeApiSvc?.CmsContext?.User?.Id ?? -1);

    #endregion

}