using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
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

    //internal required CacheContextTools CacheContextTools { get; init; }

    [field: AllowNull, MaybeNull]
    internal required CacheConfigToPolicyMaker CacheConfigToPolicyMaker { get; init; }

    //internal required CacheKeySpecs KeySpecs { get; init; }

    public CacheKeyConfig KeyConfiguration { get; init; } = new();

    public CacheWriteConfig WriteConfiguration { get; init; } = new();

    // Note: actually internal...
    [field: AllowNull, MaybeNull]
    public IPolicyMaker PolicyMaker
    {
        // Recreate whenever it is null or was reset previously
        get => field ??= CacheConfigToPolicyMaker.ReplayConfigToPolicy(KeyConfiguration, WriteConfiguration);
        internal init;
    }

    #endregion

    #region Keys

    /// <inheritdoc />
    [field: AllowNull, MaybeNull]
    public string Key
    {
        get => field ??= CacheConfigToPolicyMaker.ReplayConfigToKeySpecs(KeyConfiguration, WriteConfiguration).FinalKey;
        internal init;
    }

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
        var userElevation = CacheConfigToPolicyMaker.CacheContextTools.UserElevation;
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
        var finalSeconds = seconds ?? (int)(timeSpan ?? throw new ArgumentException("no time specified")).TotalSeconds;
        return this with
        {
            KeyConfiguration = KeyConfiguration.SetElevation(UserElevation.Any, finalSeconds),
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

    #region Watch App Data / Folder

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
    
    #endregion

    #region Vary By Value

    //public ICacheSpecs VaryBy(string value, NoParamOrder protector = default, bool caseSensitive = false)
    //    => Next(value, "", caseSensitive: caseSensitive);

    public ICacheSpecs VaryBy(string name, string value, NoParamOrder protector = default, bool caseSensitive = false)
    {
        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            WriteConfiguration = WriteConfiguration with
            {
                AdditionalValues = [.. WriteConfiguration.AdditionalValues, (name, value, caseSensitive)]
            }
        };

        //return VaryByInternal(name, value, protector, caseSensitive);
    }

    //private ICacheSpecs VaryByInternal(string name, string value, NoParamOrder protector = default, bool caseSensitive = false, string? keysForRestore = null) =>
    //    this with
    //    {
    //        Key = null!, // reset key so it will be re-calculated
    //        KeySpecs = KeySpecs.WithUpdatedVaryBy(name, value, caseSensitive),
    //        KeyConfiguration = KeyConfiguration.Updated(name, keysForRestore, caseSensitive),
    //    };

    #endregion

    #region Vary-By Custom User, QueryString, etc.

    /// <inheritdoc />
    public ICacheSpecs VaryByPageParameters(string? names = default, NoParamOrder protector = default, bool caseSensitive = false)
    {
        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            KeyConfiguration = KeyConfiguration.Updated(CacheSpecConstants.ByPageParameters, names, caseSensitive),
        };
        //var parameters = CacheContextTools.Page.Parameters ?? new Parameters { Nvc = [] };
        //var asUrl = CacheVaryByHelper.VaryByParameters(parameters, names);
        //return VaryByInternal(CacheSpecConstants.ByPageParameters, asUrl, caseSensitive: caseSensitive, keysForRestore: names);
    }

    /// <summary>
    /// Vary by Parameters is an overload we only use in testing.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="protector"></param>
    /// <param name="names"></param>
    /// <param name="caseSensitive"></param>
    /// <returns></returns>
    public ICacheSpecs VaryByParameters(IParameters parameters, NoParamOrder protector = default, string? names = default, bool caseSensitive = false)
    {
        if (!names.HasValue())
            return this;

        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            WriteConfiguration = WriteConfiguration with
            {
                AdditionalParameters = [..WriteConfiguration.AdditionalParameters, (parameters, names, caseSensitive)]
            },
        };

        //var asUrl = CacheVaryByHelper.VaryByParameters(parameters, names);
        //return VaryByInternal(CacheSpecConstants.ByParameters, asUrl, caseSensitive: caseSensitive, keysForRestore: names);
    }

    #endregion

    #region VaryByModel Experimental

    public ICacheSpecs VaryByModel(string? names = default, NoParamOrder protector = default, bool caseSensitive = false)
    {
        return !names.HasValue()
            ? this
            : this with
            {
                Key = null!, // reset key so it will be re-calculated
                KeyConfiguration = KeyConfiguration.Updated(CacheSpecConstants.ByModel, names, caseSensitive),
            };
        //var l = Log.Fn<ICacheSpecs>($"{nameof(names)}: '{names}', {nameof(caseSensitive)}: {caseSensitive}");
        //var model = CacheContextTools.Model;
        //if (model == null)
        //{
        //    // fail silently
        //    // Future: option to have aggressive mode or logging - in which case we would use the ExCtx etc. to log this message
        //    return l.Return(this, "no model, unchanged");
        //}
        //var nameList = names.CsvToArrayWithoutEmpty();
        //if (!nameList.Any())
        //    return l.Return(this, "no keys, unchanged");
        //var all = CacheVaryByModelHelper.VaryByModelExtract(model, nameList);
        //var asUrl = CacheVaryByHelper.VaryByToUrl(all);
        //var fresh = VaryByInternal(CacheSpecConstants.ByModel, asUrl, caseSensitive: caseSensitive, keysForRestore: names);
        //return l.Return(fresh, $"updated; Model had {model.Count}; used: {all.Count}");
    }


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
    public ICacheSpecs VaryByModule()
    {
        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            KeyConfiguration = KeyConfiguration with { ByModule = true }
        };
        //return VaryBy(CacheSpecConstants.ByModule, CacheContextTools.Module?.Id ?? -1);
    }

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(int id)
    //    => VaryBy(CacheSpecConstants.ByPage, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByPage(ICmsPage page)
    //    => VaryByPage(page.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByPage()
    {
        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            KeyConfiguration = KeyConfiguration with { ByPage = true }
        };
        //return VaryBy(CacheSpecConstants.ByPage, CacheContextTools.Page?.Id ?? -1);
    }

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(int id)
    //    => VaryBy(CacheSpecConstants.ByUser, id);

    ///// <inheritdoc />
    //public ICacheSpecs VaryByUser(ICmsUser user)
    //    => VaryByUser(user.Id);

    /// <inheritdoc />
    public ICacheSpecs VaryByUser()
    {
        return this with
        {
            Key = null!, // reset key so it will be re-calculated
            KeyConfiguration = KeyConfiguration with { ByUser = true }
        };
        //return VaryBy(CacheSpecConstants.ByUser, CacheContextTools.User?.Id ?? -1);
    }

    #endregion

}