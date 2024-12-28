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

namespace ToSic.Sxc.Services.Cache;

internal record CacheSpecs : ICacheSpecs
{
    #region Internal Bits to make it work

    internal required CacheKeySpecs KeySpecs { get; init; }

    internal required ICodeApiService CodeApiSvc { get; init; }

    internal required LazySvc<IAppReaderFactory> AppReaders { get; init; }

    internal required Generator<IAppPathsMicroSvc> AppPathsLazy { get; init; }

    // Note: actually internal...
    public required IPolicyMaker PolicyMaker { get; internal init; }

    #endregion

    #region Keys

    /// <inheritdoc />
    public string Key => KeySpecs.Key;

    #endregion




    #region Time Absolute / Sliding

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration) 
        => this with { PolicyMaker = PolicyMaker.SetAbsoluteExpiration(absoluteExpiration) };

    public ICacheSpecs SetSlidingExpiration(TimeSpan slidingExpiration)
        => this with { PolicyMaker = PolicyMaker.SetSlidingExpiration(slidingExpiration) };

    #endregion

    #region Watch Files / Folders - not yet on the interface, since the standard for paths is not final (eg. full path, relative path, etc.)

    public ICacheSpecs WatchFile(string filePath)
        => this with { PolicyMaker = PolicyMaker.WatchFiles([filePath]) };

    public ICacheSpecs WatchFiles(IEnumerable<string> filePaths)
        => this with { PolicyMaker = PolicyMaker.WatchFiles([..filePaths]) };

    public ICacheSpecs WatchFolder(string folderPath, bool watchSubfolders = false)
        => this with { PolicyMaker = PolicyMaker.WatchFolders(new Dictionary<string, bool> { { folderPath, watchSubfolders } }) };

    public ICacheSpecs WatchFolders(IDictionary<string, bool> folderPaths)
        => this with { PolicyMaker = PolicyMaker.WatchFolders(folderPaths) };

    #endregion

    public ICacheSpecs WatchCacheKeys(IEnumerable<string> cacheKeys)
        => this with { PolicyMaker = PolicyMaker.WatchCacheKeys(cacheKeys) };


    public ICacheSpecs WatchAppData(NoParamOrder protector = default)
        => this with { PolicyMaker = PolicyMaker.WatchNotifyKeys([AppReader.GetCache()]) };

    private IAppReader AppReader => field ??= AppReaders.Value.Get(CodeApiSvc.App.AppId);

    public ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = true)
    {
        var appPaths = AppPathsLazy.New().Get(AppReader, ((ICodeApiServiceInternal)CodeApiSvc)?._Block?.Context.Site);
        return this with
        {
            PolicyMaker = PolicyMaker.WatchFolders(new Dictionary<string, bool>
            {
                [appPaths.PhysicalPath] = withSubfolders ?? true
            })
        };
    }

    #region Vary By Value

    //public ICacheSpecs VaryBy(string value, NoParamOrder protector = default, bool caseSensitive = false)
    //    => Next(value, "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
    {
        name = "VaryBy" + name;
        var varyByKey = caseSensitive ? name : name.ToLowerInvariant();
        var valueToUse = caseSensitive ? value : value.ToLowerInvariant();

        var newDic = new Dictionary<string, string>(KeySpecs.VaryByDic ?? [], StringComparer.InvariantCultureIgnoreCase)
        {
            [varyByKey] = valueToUse
        };
        return this with { KeySpecs = KeySpecs with { VaryByDic = newDic } };
    }

    #endregion

    #region Vary-By Custom User, QueryString, etc.

    /// <inheritdoc />
    public ICacheSpecs VaryByPageParameters(string names = default, NoParamOrder protector = default, bool caseSensitive = false)
        => VaryByParamsInternal("PageParameters",
            CodeApiSvc?.CmsContext.Page.Parameters ?? new Parameters { Nvc = [] }, names,
            caseSensitive: caseSensitive
        );

    /// <inheritdoc />
    public ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string names = default, bool caseSensitive = false)
        => VaryByParamsInternal("Parameters", parameters, names, caseSensitive: caseSensitive);

    private ICacheSpecs VaryByParamsInternal(string varyByName, IParameters parameters, string names, bool caseSensitive = false)
    {
        var all = parameters
            .Filter(names)
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
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
        return VaryBy(varyByName, asUrl, caseSensitive: caseSensitive);
    }

    #endregion

    #region VaryBy Int, Page, Module, User

    /// <inheritdoc />
    public ICacheSpecs VaryBy(string name, int value) =>
        VaryBy(name, value.ToString(), caseSensitive: false);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule(int id)
        => VaryBy("Module", id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(ICmsModule module)
    //    => VaryByModule(module.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule()
        => VaryByModule(CodeApiSvc?.CmsContext.Module.Id ?? -1);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage(int id)
        => VaryBy("Page", id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(ICmsPage page)
    //    => VaryByPage(page.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage()
        => VaryByPage(CodeApiSvc?.CmsContext.Page.Id ?? -1);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser(int id)
        => VaryBy("User", id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(ICmsUser user)
    //    => VaryByUser(user.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser()
        => VaryByUser(CodeApiSvc?.CmsContext?.User?.Id ?? -1);

    #endregion

}