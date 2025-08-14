using System.Collections.Specialized;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Caching.Policies;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache;

internal record CacheSpecs : ICacheSpecs
{
    #region Internal Bits to make it work

    internal required CacheKeySpecs KeySpecs { get; init; }

    [field: AllowNull, MaybeNull]
    internal required IExecutionContext ExCtx
    {
        get => field ?? throw new NullReferenceException($"{nameof(CacheSpecs)}.{nameof(ExCtx)} should never be null at runtime, only during unit tests. Avoid test on aspects which need this.");
        init;
    }

    internal required LazySvc<IAppReaderFactory> AppReaders { get; init; }

    internal required Generator<IAppPathsMicroSvc> AppPathsLazy { get; init; }

    // Note: actually internal...
    public required IPolicyMaker PolicyMaker { get; internal init; }

    #endregion

    #region Keys

    /// <inheritdoc />
    public string Key => KeySpecs.Key;

    #endregion

    #region Enabled / disabled

    public required bool IsEnabled { get; init; }

    public ICacheSpecs Disable(NoParamOrder protector = default, UserElevation minElevation = default, UserElevation maxElevation = default)
    {
        // if for all, or not specified, then disable
        if (minElevation == default && maxElevation == default)
            return AllDisabled();

        // if no user, then disable
        var user = ExCtx.GetState<ICmsContext>()?.User;
        if (user == null)
            return AllDisabled();

        // if user elevation is in range, then disable
        var elevation = user.GetElevation();
        var isDisabled = elevation.IsForAllOrInRange(minElevation, maxElevation);

        return this with
        {
            IsEnabled = !isDisabled,
            ConfigList = ConfigList with
            {
                MinDisabledElevation = minElevation > UserElevation.Unknown ? minElevation : ConfigList.MinDisabledElevation,
                MaxDisabledElevation = maxElevation > UserElevation.Unknown ? maxElevation : ConfigList.MaxDisabledElevation
            }
        };

        ICacheSpecs AllDisabled() => this with { IsEnabled = false, ConfigList = ConfigList with { MinDisabledElevation = UserElevation.Unknown, MaxDisabledElevation = UserElevation.Unknown } };
    }

    public ICacheSpecs Enable()
        => this with
        {
            IsEnabled = true,
            ConfigList = ConfigList with
            {
                MinDisabledElevation = UserElevation.Unknown, MaxDisabledElevation = UserElevation.Unknown
            }
        };

    #endregion


    #region Time Absolute / Sliding

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration) 
        => this with { PolicyMaker = PolicyMaker.SetAbsoluteExpiration(absoluteExpiration) };

    public ICacheSpecs SetSlidingExpiration(TimeSpan? timeSpan = null, NoParamOrder protector = default, int? seconds = null)
        => seconds == null
            ? this with { PolicyMaker = PolicyMaker.SetSlidingExpiration(timeSpan ?? throw new ArgumentException("no time specified")) }
            : this with { PolicyMaker = PolicyMaker.SetSlidingExpiration(seconds.Value) };
           

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

    [field: AllowNull, MaybeNull]
    private IAppReader AppReader => field ??= ExCtx.GetState<IAppReader>();

    public ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = true)
    {
        var appPaths = AppPathsLazy.New().Get(AppReader, ExCtx.GetState<IContextOfBlock>()?.Site);
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
        => VaryByInternal(name, value, protector, caseSensitive);

    private ICacheSpecs VaryByInternal(string name, string value, NoParamOrder protector = default, bool caseSensitive = false, string? keysForRestore = null)
    {
        var varyByName = "VaryBy" + name;
        var varyByKey = caseSensitive ? varyByName : varyByName.ToLowerInvariant();
        var valueToUse = caseSensitive ? value : value.ToLowerInvariant();

        var newDic = new Dictionary<string, string>(KeySpecs.VaryByDic ?? [], StringComparer.InvariantCultureIgnoreCase)
        {
            [varyByKey] = valueToUse
        };

        var newSpecs = this with
        {
            KeySpecs = KeySpecs with { Key = null! /* requires reset */, VaryByDic = newDic },
            ConfigList = ConfigList.Updated(name, keysForRestore, caseSensitive),
        };
        return newSpecs;
    }

    public CacheSpecsConfig ConfigList { get; init; } = new();

    #endregion

    #region Vary-By Custom User, QueryString, etc.

    /// <inheritdoc />
    public ICacheSpecs VaryByPageParameters(string? names = default, NoParamOrder protector = default, bool caseSensitive = false)
        => VaryByParamsInternal(CacheSpecConstants.ByPageParameters,
            ExCtx.GetState<ICmsContext>().Page.Parameters ?? new Parameters { Nvc = [] },
            names,
            caseSensitive: caseSensitive
        );

    /// <inheritdoc />
    public ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string? names = default, bool caseSensitive = false)
        => VaryByParamsInternal(CacheSpecConstants.ByParameters, parameters, names, caseSensitive: caseSensitive);

    private ICacheSpecs VaryByParamsInternal(string varyByName, IParameters parameters, string? names, bool caseSensitive = false)
    {
        var all = parameters
            .Filter(names)
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        return VaryByParamsListKvp(all, varyByName, names, caseSensitive);
    }

    private ICacheSpecs VaryByParamsListKvp(List<KeyValuePair<string, string>> all, string varyByName, string? names,
        bool caseSensitive)
    {
        var nvc = all
            .Where(pair => pair.Value.HasValue())
            .Aggregate(new NameValueCollection(),
                (seed, pair) =>
                {
                    seed.Add(pair.Key, pair.Value);
                    return seed;
                });

        var asUrl = nvc.NvcToString();
        return VaryByInternal(varyByName, asUrl, caseSensitive: caseSensitive, keysForRestore: names);
    }

    #endregion

    #region VaryByModel Experimental

    internal IDictionary<string, object?>? Model { get; init; }

    public ICacheSpecs VaryByModel(string? names = default, NoParamOrder protector = default, bool caseSensitive = false)
    {
        if (Model == null)
            throw new InvalidOperationException("VaryByModel is not supported in this context, as Model is null. Ensure the model is set before calling this method.");

        var nameList = names.CsvToArrayWithoutEmpty();
        if (!nameList.Any())
            return this;

        var all = Model
            .Where(pair => nameList.Any(n => n.EqualsInsensitive(pair.Key)))
            .Select(p => new KeyValuePair<string, string?>(p.Key, p.Value?.ToString()))
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        return VaryByParamsListKvp(all!, CacheSpecConstants.ByModel, names, caseSensitive);
    }



    #endregion

    #region VaryBy Int, Page, Module, User

    /// <inheritdoc />
    public ICacheSpecs VaryBy(string name, int value) =>
        VaryByInternal(name, value.ToString(), caseSensitive: false);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule(int id)
        => VaryBy(CacheSpecConstants.ByModule, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(ICmsModule module)
    //    => VaryByModule(module.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule()
        => VaryByModule(ExCtx.GetState<ICmsContext>()?.Module.Id ?? -1);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage(int id)
        => VaryBy(CacheSpecConstants.ByPage, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(ICmsPage page)
    //    => VaryByPage(page.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage()
        => VaryByPage(ExCtx.GetState<ICmsContext>()?.Page.Id ?? -1);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser(int id)
        => VaryBy(CacheSpecConstants.ByUser, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(ICmsUser user)
    //    => VaryByUser(user.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser()
        => VaryByUser(ExCtx.GetState<ICmsContext>()?.User?.Id ?? -1);

    #endregion

}