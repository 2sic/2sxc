using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Services.Cache.Sys.VaryBy;
using ToSic.Sys.Caching.Policies;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Converts cache configuration to cache-policy maker.
/// </summary>
internal record CacheConfigToPolicyMaker : HelperRecordBase
{
    public required CacheContextTools CacheContextTools { get; init; }

    /// <summary>
    /// Converts cache configuration to cache-policy maker.
    /// </summary>
    public CacheConfigToPolicyMaker(ILog parentLog) : base(parentLog, "Sxc.ChPM")
    { }

    internal IPolicyMaker ReplayConfigToPolicy(CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        var policyMaker = CacheContextTools.BasePolicyMaker;
        // Only set either the absolute or sliding expiration, never both.
        policyMaker = writeConfig.AbsoluteExpiration != default
            ? policyMaker.SetAbsoluteExpiration(writeConfig.AbsoluteExpiration)
            : ApplySlidingExpiration(policyMaker, keyConfig);

        if (writeConfig.WatchAppData)
            policyMaker = policyMaker.WatchNotifyKeys([CacheContextTools.AppReader.GetCache()]);

        if (writeConfig.WatchAppFolder)
        {
            var appPaths = CacheContextTools.AppPathsLazy.New().Get(CacheContextTools.AppReader, CacheContextTools.Site);
            policyMaker = policyMaker.WatchFolders(new Dictionary<string, bool>
            {
                [appPaths.PhysicalPath] = writeConfig.WatchAppSubfolders
            });
        }

        return policyMaker;
    }

    private IPolicyMaker ApplySlidingExpiration(IPolicyMaker policyMaker, CacheKeyConfig keyConfig)
    {
        var slidingAny = keyConfig.SecondsFor(CacheContextTools.UserElevation);
        return slidingAny > 0
            ? policyMaker.SetSlidingExpiration(slidingAny)
            : policyMaker;
    }

    internal CacheKeySpecs ReplayConfigToKeySpecs(CacheKeyConfig keyConfig, CacheWriteConfig writeConfig)
    {
        var keySpecs = CacheContextTools.KeySpecs;
        if (keyConfig.ByPageParameters is { } byParameters)
        {
            var parameters = CacheContextTools.Page.Parameters ?? new Parameters { Nvc = [] };
            var asUrl = CacheVaryByHelper.VaryByParameters(parameters, byParameters.Names);
            keySpecs = keySpecs.WithUpdatedVaryBy(CacheSpecConstants.ByPageParameters, asUrl, byParameters.CaseSensitive);
        }

        foreach (var add in writeConfig.AdditionalParameters)
        {
            var asUrl = CacheVaryByHelper.VaryByParameters(add.Parameters, add.Names);
            keySpecs = keySpecs.WithUpdatedVaryBy(CacheSpecConstants.ByParameters, asUrl, add.CaseSensitive);
        }

        foreach (var add in writeConfig.AdditionalValues)
            keySpecs = keySpecs.WithUpdatedVaryBy(add.Name, add.Value, caseSensitive: false);

        if (keyConfig.ByModel is { } byModel)
            keySpecs = ReplayByModel(keySpecs, byModel.Names, caseSensitive: byModel.CaseSensitive);

        if (keyConfig.ByUser)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByUser, CacheContextTools.User?.Id);

        if (keyConfig.ByModule)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByModule, CacheContextTools.Module?.Id);

        if (keyConfig.ByPage)
            keySpecs = Update(keySpecs, CacheSpecConstants.ByPage, CacheContextTools.Page?.Id);

        // Make sure the final key is always null, so that it will be re-calculated
        // since before every copy would accidentally set it.
        keySpecs = keySpecs with { FinalKey = null! };

        return keySpecs;

        CacheKeySpecs Update(CacheKeySpecs tempSpecs, string name, int? value)
            => tempSpecs.WithUpdatedVaryBy(name, (value ?? -1).ToString(), true);
    }

    public CacheKeySpecs ReplayByModel(CacheKeySpecs keySpecs, string? names, bool caseSensitive)
    {
        var l = Log.Fn<CacheKeySpecs>($"{nameof(names)}: '{names}', {nameof(caseSensitive)}: {caseSensitive}");
        var model = CacheContextTools.Model;
        if (model == null)
        {
            // fail silently
            // Future: option to have aggressive mode or logging - in which case we would use the ExCtx etc. to log this message
            return l.Return(keySpecs, "no model, unchanged");
        }

        var nameList = names.CsvToArrayWithoutEmpty();
        if (!nameList.Any())
            return l.Return(keySpecs, "no keys, unchanged");

        var all = CacheVaryByModelHelper.VaryByModelExtract(model, nameList);

        var asUrl = CacheVaryByHelper.VaryByToUrl(all);
        keySpecs = keySpecs.WithUpdatedVaryBy(CacheSpecConstants.ByModel, asUrl, caseSensitive);
        return l.Return(keySpecs, $"updated; Model had {model.Count}; used: {all.Count}");
    }

}
