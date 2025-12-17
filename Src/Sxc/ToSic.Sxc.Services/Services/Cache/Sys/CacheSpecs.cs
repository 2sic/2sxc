using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Cache.Sys.CacheKey;
using ToSic.Sys.Caching.Policies;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// This is the "public API" for things which setup cache specs.
/// It does very little, but manage key/write configurations.
/// In the end it provides both the policy-maker and the key.
/// </summary>
internal record CacheSpecs : HelperRecordBase, ICacheSpecs
{
    public CacheSpecs(ILog parentLog) : base(parentLog, "Sxc.ChKySp")
    { }

    #region Internal Bits to make it work

    internal required CacheSpecsContextAndTools CacheSpecsContextAndTools { get; init; }

    public CacheKeyConfig KeyConfig { get; init; } = new();

    public CacheWriteConfig WriteConfig { get; init; } = new();

    // Note: actually internal...
    [field: AllowNull, MaybeNull]
    public IPolicyMaker PolicyMaker
    {
        // Recreate whenever it is null or was reset previously
        get => field ??= CacheSpecsContextAndTools.ApplyConfigToPolicy(KeyConfig, WriteConfig);
        internal init;
    }

    #endregion

    #region Keys

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Key
    {
        get => field ??= CacheSpecsContextAndTools.ApplyToKeySpecs(KeyConfig, WriteConfig).GetKey();
        internal init;
    }

    #endregion

    private CacheSpecs WithChanges(CacheKeyConfig? keyConfig = null, CacheWriteConfig? writeConfig = null) =>
        this with
        {
            Key = null!, // reset key so it will be re-calculated
            KeyConfig = keyConfig ?? KeyConfig,
            PolicyMaker = null!, // reset policy-maker so it will be re-calculated
            WriteConfig = writeConfig ?? WriteConfig,
        };

    #region Enabled / disabled

    public bool IsEnabled => KeyConfig.ForElevation.IsEnabledFor(CacheSpecsContextAndTools.UserElevation);

    public ICacheSpecs Disable() =>
        WithChanges(KeyConfig with { ForElevation = ForElevationExtensions.ResetAll(CacheKeyConfig.Disabled), });

    public ICacheSpecs Disable(UserElevation elevation) =>
        WithChanges(KeyConfig with { ForElevation = KeyConfig.ForElevation.SetForOneOrAll(elevation, CacheKeyConfig.Disabled), });

    public ICacheSpecs Disable(UserElevation minElevation, UserElevation maxElevation) =>
        WithChanges(KeyConfig with { ForElevation = KeyConfig.ForElevation.SetRange(minElevation, maxElevation, CacheKeyConfig.Disabled), });

    public ICacheSpecs Enable()
        => WithChanges(KeyConfig with { ForElevation = ForElevationExtensions.ResetAll(CacheKeyConfig.EnabledWithoutTime), });

    public ICacheSpecs Enable(UserElevation elevation) =>
        WithChanges(KeyConfig with { ForElevation = KeyConfig.ForElevation.SetForOneOrAll(elevation, CacheKeyConfig.EnabledWithoutTime), });

    public ICacheSpecs Enable(UserElevation minElevation, UserElevation maxElevation) =>
        WithChanges(KeyConfig with { ForElevation = KeyConfig.ForElevation.SetRange(minElevation, maxElevation, CacheKeyConfig.EnabledWithoutTime), });

    #endregion


    #region Time Absolute / Sliding

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration)
        => WithChanges(writeConfig: WriteConfig with { AbsoluteExpiration = absoluteExpiration });

    public ICacheSpecs SetSlidingExpiration(TimeSpan? timeSpan = null, NoParamOrder npo = default, int? seconds = null) =>
        WithChanges(keyConfig: KeyConfig with
        {
            ForElevation = KeyConfig.ForElevation.SetOne(
                UserElevation.All,
                seconds ?? (int)(timeSpan ?? throw new ArgumentException("no time specified")).TotalSeconds
            ),
        });

    #endregion

    #region Watch Files / Folders - not yet on the interface, since the standard for paths is not final (eg. full path, relative path, etc.)

    //public ICacheSpecs WatchFile(string filePath)
    //    => this with { PolicyMaker = PolicyMaker.WatchFiles([filePath]) };

    //public ICacheSpecs WatchFiles(IEnumerable<string> filePaths)
    //    => this with { PolicyMaker = PolicyMaker.WatchFiles([..filePaths]) };

    //public ICacheSpecs WatchFolder(string folderPath, bool watchSubfolders = false)
    //    => this with { PolicyMaker = PolicyMaker.WatchFolders(new Dictionary<string, bool> { { folderPath, watchSubfolders } }) };

    //public ICacheSpecs WatchFolders(IDictionary<string, bool> folderPaths)
    //    => this with { PolicyMaker = PolicyMaker.WatchFolders(folderPaths) };

    //public ICacheSpecs WatchCacheKeys(IEnumerable<string> cacheKeys)
    //    => this with { PolicyMaker = PolicyMaker.WatchCacheKeys(cacheKeys) };

    #endregion

    #region Watch App Data / Folder

    public ICacheSpecs WatchAppData(NoParamOrder npo = default) =>
        WithChanges(writeConfig: WriteConfig with { WatchAppData = true, });

    public ICacheSpecs WatchAppFolder(NoParamOrder npo = default, bool? withSubfolders = default) =>
        WithChanges(writeConfig: WriteConfig with { WatchAppFolder = true, WatchAppSubfolders = withSubfolders ?? true });
    
    #endregion

    #region Vary By Value

    //public ICacheSpecs VaryBy(string value, NoParamOrder npo = default, bool caseSensitive = false)
    //    => Next(value, "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryBy(string name, string value, NoParamOrder npo = default, bool caseSensitive = false) =>
        WithChanges(writeConfig: WriteConfig with
        {
            AdditionalValues = [.. WriteConfig.AdditionalValues, (name, value, caseSensitive)]
        });

    #endregion

    #region Vary-By Custom User, QueryString, etc.

    /// <inheritdoc />
    public ICacheSpecs VaryByPageParameters(string? names = default, NoParamOrder npo = default, bool caseSensitive = false)
        => WithChanges(KeyConfig.Updated(CacheSpecConstants.ByPageParameters, names, caseSensitive));

    /// <summary>
    /// Vary by Parameters is an overload we only use in testing.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="names"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder npo = default, string? names = default, bool caseSensitive = false) =>
        WithChanges(writeConfig: WriteConfig with
            {
                AdditionalParameters = [..WriteConfig.AdditionalParameters, (parameters, names, caseSensitive)]
            });

    #endregion

    #region VaryByModel Experimental

    public ICacheSpecs VaryByModel(string? names = default, NoParamOrder npo = default, bool caseSensitive = false) =>
        WithChanges(KeyConfig with
            {
                ByModel = CacheKeyConfigExtensions.Update(KeyConfig.ByModel, names, caseSensitive)
            });

    #endregion

    #region VaryBy Int, Page, Module, User

    /// <inheritdoc />
    public ICacheSpecs VaryBy(string name, int value)
        => VaryBy(name, value.ToString(), caseSensitive: false);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(int id)
    //    => VaryBy(CacheSpecConstants.ByModule, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(ICmsModule module)
    //    => VaryByModule(module.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule() =>
        WithChanges(KeyConfig with { ByModule = true });

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(int id)
    //    => VaryBy(CacheSpecConstants.ByPage, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(ICmsPage page)
    //    => VaryByPage(page.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage() =>
        WithChanges(KeyConfig with { ByPage = true });

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(int id)
    //    => VaryBy(CacheSpecConstants.ByUser, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(ICmsUser user)
    //    => VaryByUser(user.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser() =>
        WithChanges(KeyConfig with { ByUser = true });

    #endregion

}