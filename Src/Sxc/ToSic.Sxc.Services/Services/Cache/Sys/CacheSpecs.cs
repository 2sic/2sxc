using System.Collections.Specialized;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Caching.Policies;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;

internal record CacheSpecs : HelperRecordBase, ICacheSpecs
{
    /// <summary>
    /// Not primary constructor, because we don't want parentLog to become a property.
    /// </summary>
    /// <param name="parentLog"></param>
    public CacheSpecs(ILog parentLog) : base(parentLog, "Sxc.ChKySp")
    { }

    #region Internal Bits to make it work

    internal required CacheContextTools CacheContextTools { get; init; }

    internal required CacheKeySpecs KeySpecs { get; init; }

    public CacheKeyConfig KeyConfiguration { get; init; } = new();

    public CacheWriteConfig WriteConfiguration { get; init; } = new();

    // Note: actually internal...
    [field: AllowNull, MaybeNull]
    public IPolicyMaker PolicyMaker
    {
        // Recreate whenever it is null or was reset previously
        get => field ??= new CacheConfigToPolicyMaker(CacheContextTools).ReplayConfig(CacheContextTools.BasePolicyMaker, KeyConfiguration, WriteConfiguration);
        internal init;
    }

    #endregion

    #region Keys

    /// <inheritdoc />
    public string Key => KeySpecs.Key;

    #endregion

    #region Enabled / disabled

    public required bool IsEnabled { get; init; }

    public ICacheSpecs Disable(NoParamOrder protector = default, UserElevation minElevation = default, UserElevation maxElevation = default)
    {
        // If not specified, for all, or within the lowest and highest elevation, then disable
        if (minElevation is UserElevation.Unknown or UserElevation.Any or UserElevation.Anonymous
            && maxElevation is UserElevation.Unknown or UserElevation.Any or UserElevation.SystemAdmin)
            return AllDisabled();

        // Create a list of all elevations which should be disabled
        var listToDisable = Enum.GetValues(typeof(UserElevation))
            .Cast<UserElevation>()
            .Where(e => e <= minElevation && e >= maxElevation)
            .ToList();

        var toUpdate = new Dictionary<UserElevation, int>(KeyConfiguration.ForElevation);
        foreach (var elevation in listToDisable)
        {
            toUpdate[elevation] = CacheKeyConfig.Disabled;
        }

        // if user elevation is in range, then disable
        var userElevation = CacheContextTools.UserElevation;
        var isDisabled = userElevation.IsForAllOrInRange(minElevation, maxElevation);

        return this with
        {
            IsEnabled = !isDisabled,
            KeyConfiguration = KeyConfiguration with
            {
                ForElevation = toUpdate,
            }
        };

        ICacheSpecs AllDisabled() => this with
        {
            IsEnabled = false,
            KeyConfiguration = KeyConfiguration with
            {
                ForElevation = new() { [UserElevation.Any] = CacheKeyConfig.Disabled },
            }
        };
    }

    public ICacheSpecs Enable()
        => this with
        {
            IsEnabled = true,
            KeyConfiguration = KeyConfiguration with
            {
                ForElevation = new() { [UserElevation.Any] = 0 },
            }
        };

    #endregion


    #region Time Absolute / Sliding

    public ICacheSpecs SetAbsoluteExpiration(DateTimeOffset absoluteExpiration) 
        => this with
        {
            WriteConfiguration = WriteConfiguration with { AbsoluteExpiration = absoluteExpiration },
            PolicyMaker = null!,
        };

    public ICacheSpecs SetSlidingExpiration(TimeSpan? timeSpan = null, NoParamOrder protector = default, int? seconds = null)
    {
        var newConfig = seconds == null
            ? KeyConfiguration.SetElevation(UserElevation.Any, (int)(timeSpan ?? throw new ArgumentException("no time specified")).TotalSeconds)
            : KeyConfiguration.SetElevation(UserElevation.Any, seconds.Value);

        return this with
        {
            KeyConfiguration = newConfig,
            PolicyMaker = null!,
        };

    }

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


    public ICacheSpecs WatchAppData(NoParamOrder protector = default) =>
        this with
        {
            PolicyMaker = null!,
            WriteConfiguration = WriteConfiguration with { WatchAppData = true, },
        };

    public ICacheSpecs WatchAppFolder(NoParamOrder protector = default, bool? withSubfolders = default) =>
        this with
        {
            PolicyMaker = null!,
            WriteConfiguration = WriteConfiguration with { WatchAppFolder = true, WatchAppSubfolders = withSubfolders ?? true },
        };

    #region Vary By Value

    //public ICacheSpecs VaryBy(string value, NoParamOrder protector = default, bool caseSensitive = false)
    //    => Next(value, "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
        => VaryByInternal(name, value, protector, caseSensitive);

    private ICacheSpecs VaryByInternal(string name, string value, NoParamOrder protector = default, bool caseSensitive = false, string? keysForRestore = null) =>
        this with
        {
            KeySpecs = KeySpecs.VaryBy(name, value, caseSensitive),
            KeyConfiguration = KeyConfiguration.Updated(name, keysForRestore, caseSensitive),
        };

    #endregion

    #region Vary-By Custom User, QueryString, etc.

    // INTERNAL!
    /// <inheritdoc />
    public ICacheSpecs VaryByPageParameters(string? names = default, NoParamOrder protector = default, bool caseSensitive = false)
        => VaryByParamsInternal(CacheSpecConstants.ByPageParameters,
            CacheContextTools.Page.Parameters ?? new Parameters { Nvc = [] },
            names,
            caseSensitive: caseSensitive
        );

    /// <summary>
    /// Vary by Parameters is an overload we only use in testing.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="protector"></param>
    /// <param name="names"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
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
        var l = Log.Fn<ICacheSpecs>(names);
        if (Model == null)
        {
            // fail silently
            // Future: option to have aggressive mode or logging - in which case we would use the ExCtx etc. to log this message
            return l.Return(this, "no model, unchanged");
        }

        var nameList = names.CsvToArrayWithoutEmpty();
        if (!nameList.Any())
            return l.Return(this, "no keys, unchanged");

        var all = Model
            .Where(pair => nameList.Any(n
                => n.EqualsInsensitive(pair.Key)                    // contains key
                   && IsUsefulForCacheKey(pair.Value)     // is simple value, allowing use in cache key
            ))
            .Select(p => new KeyValuePair<string, string?>(p.Key, p.Value?.ToString()))
            .OrderBy(p => p.Key, comparer: StringComparer.InvariantCultureIgnoreCase)
            .ToList();

        var fresh = VaryByParamsListKvp(all!, CacheSpecConstants.ByModel, names, caseSensitive);
        return l.Return(fresh, $"updated; Model had {Model.Count}; used: {all.Count}");
    }

    private static bool IsUsefulForCacheKey(object? value)
    {
        if (value == null)
            return false;
        var type = value.GetType().UnboxIfNullable();
        return type.IsValueType
               || type.IsPrimitive
               || type == typeof(string)
               || type == typeof(DateTime)
               || type == typeof(Guid)
               || type.IsNumeric();
    }


    #endregion

    #region VaryBy Int, Page, Module, User

    /// <inheritdoc />
    public ICacheSpecs VaryBy(string name, int value) =>
        VaryByInternal(name, value.ToString(), caseSensitive: false);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(int id)
    //    => VaryBy(CacheSpecConstants.ByModule, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByModule(ICmsModule module)
    //    => VaryByModule(module.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByModule()
        => VaryBy(CacheSpecConstants.ByModule, CacheContextTools.Module?.Id ?? -1);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(int id)
    //    => VaryBy(CacheSpecConstants.ByPage, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(ICmsPage page)
    //    => VaryByPage(page.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage()
        => VaryBy(CacheSpecConstants.ByPage, CacheContextTools.Page?.Id ?? -1);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(int id)
    //    => VaryBy(CacheSpecConstants.ByUser, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(ICmsUser user)
    //    => VaryByUser(user.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser()
        => VaryBy(CacheSpecConstants.ByUser, CacheContextTools.User?.Id ?? -1);

    #endregion

}