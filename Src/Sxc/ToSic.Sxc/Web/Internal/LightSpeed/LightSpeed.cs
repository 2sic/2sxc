using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Caching;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Internal.Generics;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;

namespace ToSic.Sxc.Web.Internal.LightSpeed;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LightSpeed(
    IEavFeaturesService features,
    LazySvc<IAppsCatalog> appsCatalog,
    LazySvc<IAppReaderFactory> appReadersLazy,
    Generator<IAppPathsMicroSvc> appPathsLazy,
    LazySvc<ICmsContext> cmsContext,
    LazySvc<OutputCacheManager> outputCacheManager
) : ServiceBase(SxcLogName + ".Lights", connect: [features, appsCatalog, appReadersLazy, appPathsLazy, cmsContext, outputCacheManager]), IOutputCache
{
    public IOutputCache Init(int moduleId, int pageId, IBlock block)
    {
        var l = Log.Fn<IOutputCache>($"mod: {moduleId}");
        _moduleId = moduleId;
        _pageId = pageId;
        _block = block;
        bool isEnabled;
        try
        {
            isEnabled = IsEnabled;
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return(this, "exception during init");
        }
        return l.Return(this, $"{isEnabled}");
    }
    private int _moduleId;
    private int _pageId;
    private IBlock _block;
    private IAppReader AppReader => field ??= _block?.Context?.AppReader;

    public bool Save(IRenderResult data) => AddToLightSpeed(data);

#if NETFRAMEWORK
    public bool Save(IRenderResult data, bool enforcePre1025)
        => AddToLightSpeed(data, cacheData => cacheData.EnforcePre1025 = enforcePre1025);
#endif

    public bool AddToLightSpeed(IRenderResult data, Action<OutputCacheItem> doOtherStuff = null)
    {
        var l = Log.Fn<bool>(timer: true);

        // Check many exit-early clauses
        try
        {
            if (!IsEnabled) return l.ReturnFalse("disabled");
            if (data == null) return l.ReturnFalse("null");
            if (data.IsError) return l.ReturnFalse("error");
            if (!data.CanCache) return l.ReturnFalse("can't cache");
            if (data.OutputCacheSettings?.IsEnabled == false)
                return l.ReturnFalse("disabled in settings from code"); // new v19.03.03
            if (data == Existing?.Data) return l.ReturnFalse("not new");
            if (data.DependentApps.SafeNone()) return l.ReturnFalse("app not initialized");
            if (!UrlParams.CachingAllowed) return l.ReturnFalse("url params not allowed");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("exception during exit-early clauses");
        }

        // Figure out dependent apps
        List<IAppReader> dependentAppsStates;
        try
        {
            // get dependent appStates
            dependentAppsStates = data.DependentApps
                .Select(da => appReadersLazy.Value.Get(da.AppId))
                .ToList();
            l.A($"{nameof(dependentAppsStates)} count {dependentAppsStates.Count}");

            // when dependent apps have disabled caching, parent app should not cache also 
            if (!IsEnabledOnDependentApps(dependentAppsStates))
                return l.ReturnFalse("disabled in dependent app");

            // respect primary app (of site) as dependent app to ensure cache invalidation when primary app is changed
            var appState = AppReader;
            if (appState == null)
                return l.ReturnFalse("no app");

            if (appState.ZoneId >= 0)
            {
                l.A("dependentAppsStates add");
                var primary = appsCatalog.Value.PrimaryAppIdentity(appState.ZoneId);
                dependentAppsStates.Add(appReadersLazy.Value.Get(primary));
            }

            l.A($"Found {data.DependentApps.Count} apps: " +
                string.Join(",", data.DependentApps.Select(da => da.AppId)));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("exception during dependent apps");
        }

        // Add to cache
        try
        {
            var cacheItem = new OutputCacheItem(data);
            doOtherStuff?.Invoke(cacheItem);

            var duration = Duration;
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return l.ReturnFalse($"not added as duration is {duration}");

            var appPathsToMonitor = features.IsEnabled(LightSpeedOutputCacheAppFileChanges.NameId)
                ? _appPaths.Get(() => AppPaths(dependentAppsStates))
                : null;
            l.A($"{nameof(appPathsToMonitor)} done");

            // add to cache and log
            string cacheKey = null;
            l.Do(message: "outputCacheManager add", timer: true,
                action: () => cacheKey = OutCacheMan.Add(
                    CacheKey,
                    cacheItem,
                    duration,
                    dependentAppsStates.Select(r => r.GetCache()).Cast<ICanBeCacheDependency>().ToList(),
                    appPathsToMonitor
                )
            );

            return l.ReturnTrue($"added for {duration}s; cache key: '{cacheKey}'");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("exception during add to cache");
        }
    }

    /// <summary>
    /// find if caching is enabled on all dependent apps
    /// </summary>
    private bool IsEnabledOnDependentApps(List<IAppReader> appStates)
    {
        var l = Log.Fn<bool>(timer: true);
        var appWhereNotEnabled = appStates
            .FirstOrDefault(appState => !GetLightSpeedConfigOfApp(appState).IsEnabled);

        return appWhereNotEnabled != null
            ? l.ReturnFalse($"Can't cache; caching disabled on dependent app {appWhereNotEnabled.AppId}")
            : l.ReturnTrue("ok");

        // old code before 2025-03-17 - remove old code if all ok by 2025-Q3
        //foreach (var appState in appStates.Where(appState => !GetLightSpeedConfig(appState).IsEnabled))
        //    return l.ReturnFalse($"Can't cache; caching disabled on dependent app {appState.AppId}");
        //return l.ReturnTrue("ok");
    }

    /// <summary>
    /// Get physical paths for parent app and all dependent apps (portal and shared)
    /// </summary>
    /// <remarks>
    /// Note: The App Paths are only the apps in /2sxc (global and per portal)
    /// ADAM folders are not monitored
    /// </remarks>
    /// <returns>list of paths to monitor</returns>
    private IList<string> AppPaths(List<IAppReader> dependentApps)
    {
        if ((_block as BlockFromModule)?.App is not EavApp app)
            return null;
        if (dependentApps.SafeNone())
            return null;

        var paths = new List<string>();
        foreach (var appState in dependentApps)
        {
            var appPaths = appPathsLazy.New().Get(appState, app.Site);
            if (Directory.Exists(appPaths.PhysicalPath)) paths.Add(appPaths.PhysicalPath);
            if (Directory.Exists(appPaths.PhysicalPathShared)) paths.Add(appPaths.PhysicalPathShared);
        }

        return paths;
    }
    private readonly GetOnce<IList<string>> _appPaths = new();

    private int Duration => _duration ??= _block.Context.User switch
    {
        { IsSystemAdmin: true } => AppConfig.DurationSystemAdmin,
        { IsSiteAdmin: true } => AppConfig.DurationEditors,
        { IsAnonymous: false } => AppConfig.DurationUsers,
        _ => AppConfig.Duration
    };
    private int? _duration;

    private (bool CachingAllowed, string Extension) UrlParams => _urlParams.Get(() =>
        LightSpeedUrlParams.GetUrlParams(ViewConfigOrNull ?? AppConfig, _block.Context.Page.Parameters, Log)
    );
    private readonly GetOnce<(bool CachingAllowed, string Extension)> _urlParams = new();
    
    private string CurrentCulture => _currentCulture.Get(() => cmsContext.Value.Culture.CurrentCode);
    private readonly GetOnce<string> _currentCulture = new();


    private string CacheKey => _key.Get(() => Log.Func(() => OutputCacheManager.Id(_moduleId, _pageId, UserIdOrAnon, ViewKey, UrlParams.Extension, CurrentCulture)));
    private readonly GetOnce<string> _key = new();

    private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? null : _block.Context.User.Id);
    private readonly GetOnce<int?> _userId = new();

    // Note 2023-10-30 2dm changed the handling of the preview template and checks if it's set. In case caching is too aggressive this can be the problem. Remove early 2024
    private string ViewKey => _viewKey.Get(() => _block.Configuration?.PreviewViewEntity != null
        ? $"{_block.Configuration.AppId}:{_block.Configuration.View?.Id}"
        : null
    );
    private readonly GetOnce<string> _viewKey = new();

    public OutputCacheItem Existing => _existing.Get(GetExisting);
    private readonly GetOnce<OutputCacheItem> _existing = new();

    private OutputCacheItem GetExisting()
    {
        var l = Log.Fn<OutputCacheItem>();
        try
        {
            // If App not known, it can't have a cache - exit early
            if (AppReader == null)
                return l.ReturnNull("no app");

            // If Cache is enabled, try to get
            var result = IsEnabled
                ? OutCacheMan.Get(CacheKey)
                : null;

            // Not found, exit
            if (result == null)
                return l.ReturnNull("not in cache");

            // This is a bit unclear - it seems that only if dependent apps are registered, will the cache be treated as valid...?
            // compare cache time-stamps
            var dependentApp = result.Data?.DependentApps?.FirstOrDefault();
            return dependentApp == null
                ? l.ReturnNull("no dep app")
                : l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }


    public bool IsEnabled => _enabled.Get(GetIsEnabled);
    private readonly GetOnce<bool> _enabled = new();

    private bool GetIsEnabled()
    {
        var l = Log.Fn<bool>();

        // This is called from outside, so we need to catch all exceptions as it should never break in production
        try
        {
            // special - Oqtane seems to call this much earlier than Dnn, even on non-existing modules.
            // so on new modules this would fail and throw an error. So we'll just return false in this case.
            if (AppReader == null)
                return l.ReturnFalse("disabled, no app");

            // Normal check.
            l.A("before features check");
            var feat = features.IsEnabled(LightSpeedOutputCache.NameId);
            l.A($"features: {feat}");
            if (!feat)
                return l.ReturnFalse("disabled, feature");

            l.A("before app config check");

            if (!AppConfig.IsEnabled)
                return l.ReturnFalse("disabled, app not enabled");

            l.A("before view config check");
            if (ViewConfigOrNull?.IsEnabledNullable == false)
                return l.ReturnFalse("disabled at view explicit");

            l.A("before url params check");
            if (!UrlParams.CachingAllowed)
                return l.ReturnFalse("disabled, url params not allowed");

            l.A("all done");
            return l.ReturnTrue("enabled");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse("exception");
        }
    }

    /// <summary>
    /// Lightspeed Configuration at App Level
    /// </summary>
    private LightSpeedDecorator AppConfig => _lsd.Get(() => GetLightSpeedConfigOfApp(AppReader));
    private readonly GetOnce<LightSpeedDecorator> _lsd = new();

    /// <summary>
    /// Lightspeed Configuration at View Level
    /// </summary>
    private LightSpeedDecorator ViewConfigOrNull => _viewConfig.Get(() =>
        _block.View?.Metadata
            .OfType(LightSpeedDecorator.TypeNameId)
            .FirstOrDefault()
            .NullOrGetWith(viewLs => new LightSpeedDecorator(viewLs))
    );
    private readonly GetOnce<LightSpeedDecorator> _viewConfig = new();

    private LightSpeedDecorator GetLightSpeedConfigOfApp(IAppReader appReader)
    {
        var l = Log.Fn<LightSpeedDecorator>();
        var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader/*, Log*/);
        return l.Return(decoFromPiggyBack, $"has decorator: {decoFromPiggyBack.Entity != null}");
    }

    private OutputCacheManager OutCacheMan => outputCacheManager.Value;

}