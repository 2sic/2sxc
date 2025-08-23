using System.Diagnostics.CodeAnalysis;
using ToSic.Sxc.Cms.Users;
using ToSic.Sxc.Cms.Users.Sys;
using ToSic.Sxc.Code.Razor.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Cache.Sys;
using ToSic.Sxc.Services.Cache.Sys.CacheKey;
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
        ??= CacheSvc.CreateSpecs(CacheSpecConstants.PrefixForDontPrefix + OutputCacheKeys.PartialKey(appId, normalizedPath))
            .AttachModel(model);

    /// <summary>
    /// The specs for the partial rendering - these may get changed by the Razor at runtime, so create once and keep it mutable.
    /// </summary>
    [field: AllowNull, MaybeNull]
    public RenderPartialSpecsWithCaching RenderPartialSpecsForRazor => field ??= new()
    {
        CacheSpecs = CacheSpecsRawWithModel.Disable(),
    };

    [field: AllowNull, MaybeNull]
    private ICacheSpecs CacheSpecsForSettings => field
        ??= CacheSvc.CreateSpecs(CacheSpecConstants.PrefixForDontPrefix + OutputCacheKeys.PartialSettingsKey(appId, normalizedPath));

    private CacheKeyConfig? CacheSpecsConfig => _cacheSpecsConfig.Get(() => CacheSvc.Get<CacheKeyConfig>(CacheSpecsForSettings));
    private readonly GetOnce<CacheKeyConfig?> _cacheSpecsConfig = new();

    private ICacheSpecs? GetSpecsBasedOnSettings()
    {
        var l = Log.Fn<ICacheSpecs?>();
        var config = CacheSpecsConfig;
        if (config == null)
            return l.ReturnNull("settings for partial not in cache");

        var foundation = config.RestoreBy(CacheSpecsRawWithModel);

        return l.Return(foundation, $"CacheKey to look for: '{foundation.Key}'");
    }

    /// <summary>
    /// Try to get the data from the cache.
    /// </summary>
    /// <returns></returns>
    public IRenderResult? TryGetFromCache()
    {
        var l = Log.Fn<IRenderResult?>();
        if (!IsEnabled)
            return l.ReturnNull("feature not enabled");


        // Check if it's disabled for this elevation, in which case we should not pick it up
        var config = CacheSpecsConfig;
        if (config == null)
            return AttachListenerAndExit("no config");
        var user = exCtx.GetState<ICmsContext>().User;
        var elevation = user.GetElevation();
        if (!config.IsEnabledFor(elevation))
            return AttachListenerAndExit($"user elevation '{elevation.ToString()}' not enabled, ignore cache.");

        var specsBasedOnSettings = GetSpecsBasedOnSettings();
        if (specsBasedOnSettings == null)
            return AttachListenerAndExit("no settings");

        var cached = CacheSvc.Get<OutputCacheItem>(specsBasedOnSettings);
        return cached == null
            ? AttachListenerAndExit("not cached") :
            // If we have a cached result, return it
            l.Return(cached.Data, "is cached");

        IRenderResult? AttachListenerAndExit(string message)
        {
            // WIP if it is enabled, we should attach a listener
            Listener = PageService.Listeners.CreateRenderListener();
            return l.ReturnNull(message);
        }
    }

    public bool IsFullyEnabled => IsEnabled && RenderPartialSpecsForRazor.CacheSpecs.IsEnabled;

    public bool SaveToCacheIfEnabled(string html)
    {
        var l = Log.Fn<bool>();
        var partialRenderSpecs = RenderPartialSpecsForRazor.CacheSpecs;
        if (!IsFullyEnabled)
            return l.ReturnFalse("no partial caching");

        l.A($"Add to cache");
        CacheSvc.Set(partialRenderSpecs, new OutputCacheItem((Listener ?? new RenderResult()) with 
        {
            AppId = appId,
            Html = html,
            IsPartial = true,
        }));

        // detach listener if it exists, so it doesn't get saved to cache
        if (Listener != null)
        {
            l.A("detaching listener");
            PageService.Listeners.RemoveListener(Listener);
        }

        // also add the configuration to the cache, so it can decide which specs to use next time
        var storedConfig = CacheSpecsConfig;
        var newConfig = partialRenderSpecs.GetConfig();
        if (storedConfig == newConfig)
            return l.ReturnTrue("Saved to cache; config unchanged"); // only update cached config if it changed

        // If the config changed, we need to
        // - create fresh settings specs which will be persisted with the same policy as the data we just saved
        // - save the config for the settings
        var newCacheSpecsForSettings = CacheSpecsForSettings.WithPolicyOf(partialRenderSpecs);
        CacheSvc.Set(newCacheSpecsForSettings, newConfig);
        return l.ReturnTrue("Saved to cache, config updated");
    }

    #region WIP Listener

    [field: AllowNull, MaybeNull]
    public Services.Page.Sys.PageService PageService =>
        field ??= (Services.Page.Sys.PageService)exCtx.GetService<Services.IPageService>(reuse: true);
    public RenderResult? Listener { get; set; }

    #endregion
}
