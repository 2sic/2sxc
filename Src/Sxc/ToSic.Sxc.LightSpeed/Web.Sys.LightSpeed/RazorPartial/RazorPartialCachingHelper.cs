using System.Diagnostics.CodeAnalysis;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Context;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

/// <summary>
/// Helper to manage Razor partial caching.
/// </summary>
/// <param name="normalizedPath"></param>
/// <param name="exCtx"></param>
/// <param name="featureSvc"></param>
/// <param name="parentLog"></param>
public class RazorPartialCachingHelper(int appId, string normalizedPath, IDictionary<string, object?>? model, IExecutionContext exCtx, IFeaturesService featureSvc, ILog parentLog) : HelperBase(parentLog, "Rzr.Cache")
{

    private bool IsEnabled => _isEnabled ??= featureSvc.IsEnabled(SxcFeatures.LightSpeedOutputCachePartials.NameId);
    private bool? _isEnabled;

    /// <summary>
    /// Underlying cache service, taken from the execution context so it knows more about the current request.
    /// </summary>
    [field: AllowNull, MaybeNull]
    private ICacheService CacheSvc => field ??= exCtx.GetService<ICacheService>();

    /// <summary>
    /// Cache specs prepared for the current partial rendering - contains the path.
    /// </summary>
    // [field: AllowNull, MaybeNull]
    [field: AllowNull, MaybeNull]
    public ICacheSpecs CacheSpecsRawWithModel => field
        ??= CacheSvc.CreateSpecs("***" + OutputCacheKeys.PartialKey(appId, normalizedPath))
            .AttachModel(model);

    

    [field: AllowNull, MaybeNull]
    private ICacheSpecs SettingsSpecs => field
        ??= CacheSvc.CreateSpecs("***" + OutputCacheKeys.PartialSettingsKey(appId, normalizedPath));

    private CacheSpecsConfig? CacheSpecsConfig => _cacheSpecsConfig.Get(() => CacheSvc.Get<CacheSpecsConfig>(SettingsSpecs));
    private readonly GetOnce<CacheSpecsConfig?> _cacheSpecsConfig = new();

    private ICacheSpecs? GetSpecsBasedOnSettings()
    {
        var l = Log.Fn<ICacheSpecs?>();
        var config = CacheSpecsConfig;
        if (config == null)
            return l.ReturnNull("settings for partial not in cache");

        var foundation = config.Restore(CacheSpecsRawWithModel);

        return l.Return(foundation, $"CacheKey to look for: '{foundation.Key}'");
    }

    /// <summary>
    /// Try to get the data from the cache.
    /// </summary>
    /// <returns></returns>
    public string? TryGetFromCache()
    {
        var l = Log.Fn<string?>();
        if (!IsEnabled)
            return l.ReturnNull("feature not enabled");

        // Check if it's disabled for this elevation, in which case we should not pick it up
        var config = CacheSpecsConfig;
        if (config == null)
            return l.ReturnNull("no config");
        var user = exCtx.GetState<ICmsContext>().User;
        var elevation = user.GetElevation();
        if (elevation.IsForAllOrInRange(config.MinDisabledElevation, config.MaxDisabledElevation))
            return l.ReturnNull($"user elevation '{elevation.ToString()}' in cache-disabled range {config.MinDisabledElevation.ToString()} and {config.MaxDisabledElevation.ToString()}, ignore cache.");

        var specsBasedOnSettings = GetSpecsBasedOnSettings();
        if (specsBasedOnSettings == null)
            return l.ReturnNull("no settings");

        var cached = CacheSvc.Get<OutputCacheItem>(specsBasedOnSettings);
        return cached == null
            ? l.ReturnNull("not cached") :
            // If we have a cached result, return it
            l.Return(cached.Data.Html, "is cached");
    }

    public bool SaveToCache(string html, ICacheSpecs partialSpecs)
    {
        var l = Log.Fn<bool>();
        if (!IsEnabled || !partialSpecs.IsEnabled)
            return l.ReturnFalse("no partial caching");

        l.A($"Add to cache");
        CacheSvc.Set(partialSpecs, new OutputCacheItem(new RenderResult { AppId = appId, Html = html, IsPartial = true }));

        // also add the configuration to the cache, so it can decide which specs to use next time
        var storedConfig = CacheSpecsConfig;
        var newConfig = partialSpecs.GetConfig();
        if (storedConfig == newConfig)
            return l.ReturnTrue("Saved to cache; config unchanged"); // only update cached config if it changed

        // If the config changed, we need to merge it with the existing config
        var configSpecs = SettingsSpecs.MergePolicy(partialSpecs);
        CacheSvc.Set(configSpecs, partialSpecs.GetConfig());
        return l.ReturnTrue("Saved to cache, config updated");
    }
}
