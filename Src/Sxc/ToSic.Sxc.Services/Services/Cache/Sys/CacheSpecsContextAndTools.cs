using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Services.Cache.Sys.CacheKey;
using ToSic.Sxc.Services.Cache.Sys.VaryBy;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Caching.Policies;
using ToSic.Sys.Users;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Converts cache configuration to cache-policy maker.
/// </summary>
internal record CacheSpecsContextAndTools : HelperRecordBase
{
    public CacheSpecsContextAndTools(ILog parentLog) : base(parentLog, "Sxc.ChPM") { }

    #region Internal Bits / Context to make it work

    //internal required CacheKeySpecs KeySpecs { get; init; }

    [field: AllowNull, MaybeNull]
    internal required IExecutionContext ExCtx
    {
        get => field ?? throw new NullReferenceException($"{nameof(CacheSpecs)}.{nameof(ExCtx)} should never be null at runtime, only during unit tests. Avoid test on aspects which need this.");
        init;
    }

    internal required Generator<IAppPathsMicroSvc> AppPathsLazy { get; init; }

    internal required CacheKeyParts BaseKeyParts { get; init; }

    // Note: actually internal...
    public required IPolicyMaker BasePolicyMaker { get; internal init; }

    internal IDictionary<string, object?>? Model { get; init; }

    #endregion

    #region Retrieved / Calculated values for Re-Use

    [field: AllowNull, MaybeNull]
    public ICmsUser User => field ??= ExCtx.GetCmsContext().User;

    public UserElevation UserElevation => _userElevation ??= User?.GetElevation() ?? UserElevation.Unknown;
    private UserElevation? _userElevation;

    [field: AllowNull, MaybeNull]
    public ICmsModule Module => field ??= ExCtx.GetCmsContext().Module;

    [field: AllowNull, MaybeNull]
    public ICmsPage Page => field ??= ExCtx.GetCmsContext().Page;

    public ISite? Site => field ??= ExCtx.GetContextOfBlock()?.Site;

    [field: AllowNull, MaybeNull]
    internal IAppReader AppReader => field ??= ExCtx.GetState<IAppReader>();

    #endregion


    internal IPolicyMaker ApplyConfigToPolicy(CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        var policyMaker = BasePolicyMaker;
        // Only set either the absolute or sliding expiration, never both.
        policyMaker = writeConfig.AbsoluteExpiration != default
            ? policyMaker.SetAbsoluteExpiration(writeConfig.AbsoluteExpiration)
            : ApplySlidingExpiration(policyMaker, keyConfig);

        if (writeConfig.WatchAppData)
            policyMaker = policyMaker.WatchNotifyKeys([AppReader.GetCache()]);

        if (writeConfig.WatchAppFolder)
        {
            var appPaths = AppPathsLazy.New().Get(AppReader, Site);
            policyMaker = policyMaker.WatchFolders(new Dictionary<string, bool>
            {
                [appPaths.PhysicalPath] = writeConfig.WatchAppSubfolders
            });
        }

        return policyMaker;
    }

    private IPolicyMaker ApplySlidingExpiration(IPolicyMaker policyMaker, CacheKeyConfig keyConfig)
    {
        var slidingAny = keyConfig.ForElevation.SecondsFor(UserElevation);
        return slidingAny > 0
            ? policyMaker.SetSlidingExpiration(slidingAny)
            : policyMaker;
    }

    internal CacheKeyParts ApplyToKeySpecs(CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        var keySpecs = BaseKeyParts;
        if (keyConfig.ByPageParameters is { } byParameters)
        {
            var parameters = Page.Parameters ?? new Parameters { Nvc = [] };
            var asUrl = CacheVaryByHelper.VaryByParameters(parameters, byParameters.Names);
            keySpecs = keySpecs.WithUpdatedVaryBy(CacheSpecConstants.ByPageParameters, asUrl, byParameters.CaseSensitive);
        }

        foreach (var add in writeConfig.AdditionalParameters)
        {
            var asUrl = CacheVaryByHelper.VaryByParameters(add.Parameters, add.Names);
            keySpecs = keySpecs.WithUpdatedVaryBy(CacheSpecConstants.ByParameters, asUrl, add.CaseSensitive);
        }

        foreach (var add in writeConfig.AdditionalValues)
            keySpecs = keySpecs.WithUpdatedVaryBy(add.Name, add.Value, caseSensitive: add.CaseSensitive);

        if (keyConfig.ByModel is { } byModel)
            keySpecs = ReplayByModel(keySpecs, byModel.Names, caseSensitive: byModel.CaseSensitive);

        if (keyConfig.ByUser)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByUser, User?.Id);

        if (keyConfig.ByModule)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByModule, Module?.Id);

        if (keyConfig.ByPage)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByPage, Page?.Id);

        return keySpecs;

        // Helper Function to update the key specs
        CacheKeyParts Update(CacheKeyParts tempParts, string name, int? value)
            => tempParts.WithUpdatedVaryBy(name, (value ?? -1).ToString(), true);
    }

    private CacheKeyParts ReplayByModel(CacheKeyParts keyParts, string? names, bool caseSensitive)
    {
        var l = Log.Fn<CacheKeyParts>($"{nameof(names)}: '{names}', {nameof(caseSensitive)}: {caseSensitive}");
        var model = Model;
        if (model == null)
        {
            // fail silently
            // Future: option to have aggressive mode or logging - in which case we would use the ExCtx etc. to log this message
            return l.Return(keyParts, "no model, unchanged");
        }

        var nameList = names.CsvToArrayWithoutEmpty();
        if (!nameList.Any())
            return l.Return(keyParts, "no keys, unchanged");

        var all = CacheVaryByModelHelper.VaryByModelExtract(model, nameList);

        var asUrl = CacheVaryByHelper.VaryByToUrl(all);
        keyParts = keyParts.WithUpdatedVaryBy(CacheSpecConstants.ByModel, asUrl, caseSensitive);
        return l.Return(keyParts, $"updated; Model had {model.Count}; used: {all.Count}");
    }

}
