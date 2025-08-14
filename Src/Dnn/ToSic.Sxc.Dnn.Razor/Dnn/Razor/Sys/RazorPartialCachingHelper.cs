using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Helper to manage Razor partial caching.
/// </summary>
/// <param name="normalizedPath"></param>
/// <param name="exCtx"></param>
/// <param name="featureSvc"></param>
/// <param name="parentLog"></param>
internal class RazorPartialCachingHelper(string normalizedPath, IExecutionContext exCtx, IFeaturesService featureSvc, ILog parentLog) : HelperBase(parentLog, "Rzr.Cache")
{
    /// <summary>
    /// App Id
    /// </summary>
    private int AppId => _appId ??= exCtx.GetAppId();
    private int? _appId;

    private bool IsEnabled => _isEnabled ??= featureSvc.IsEnabled(SxcFeatures.LightSpeedOutputCachePartials.NameId);
    private bool? _isEnabled;

    /// <summary>
    /// Underlying cache service, taken from the execution context so it knows more about the current request.
    /// </summary>
    private ICacheService CacheSvc => field ??= exCtx.GetService<ICacheService>();

    /// <summary>
    /// Cache specs prepared for the current partial rendering - contains the path.
    /// </summary>
    // [field: AllowNull, MaybeNull]
    public ICacheSpecs CacheSpecsRaw => field
        ??= CacheSvc.CreateSpecs("***" + OutputCacheKeys.PartialKey(AppId, normalizedPath));

    // [field: AllowNull, MaybeNull]
    private ICacheSpecs SettingsSpecs => field
        ??= CacheSvc.CreateSpecs("***" + OutputCacheKeys.PartialSettingsKey(AppId, normalizedPath));

    private ICacheSpecs? GetSpecsBasedOnSettings()
    {
        var l = Log.Fn<ICacheSpecs?>();
        var setting = CacheSvc.Get<CacheSpecsVaryBy>(SettingsSpecs);
        if (setting == null)
            return l.ReturnNull("settings for partial not in cache");

        var foundation = setting.Restore(CacheSpecsRaw);
        _specsWereInCache = true;

        return l.Return(foundation, $"CacheKey to look for: '{foundation.Key}'");
    }

    private bool _specsWereInCache;

    /// <summary>
    /// Try to get the data from the cache.
    /// </summary>
    /// <returns></returns>
    public string? TryGetFromCache()
    {
        var l = Log.Fn<string>();
        if (!IsEnabled)
            return l.ReturnNull("feature not enabled");

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
        CacheSvc.Set(partialSpecs, new OutputCacheItem(new RenderResult { AppId = AppId, Html = html, IsPartial = true }));

        // also add the configuration to the cache, so it can decide which specs to use next time
        if (!_specsWereInCache)
        {
            var configSpecs = SettingsSpecs.MergePolicy(partialSpecs);
            CacheSvc.Set(configSpecs, partialSpecs.GetVaryByList());
        }

        return l.ReturnTrue("Saved to cache");
    }
}
