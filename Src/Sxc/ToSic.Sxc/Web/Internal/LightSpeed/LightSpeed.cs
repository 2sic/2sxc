using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Caching;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
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
        return l.Return(this, $"{IsEnabled}");
    }
    private int _moduleId;
    private int _pageId;
    private IBlock _block;
    private IAppReader AppState => _block?.Context?.AppReader;

    public bool Save(IRenderResult data) => AddToLightSpeed(data);

#if NETFRAMEWORK
    public bool Save(IRenderResult data, bool enforcePre1025)
        => AddToLightSpeed(data, cacheData => cacheData.EnforcePre1025 = enforcePre1025);
#endif

    public bool AddToLightSpeed(IRenderResult data, Action<OutputCacheItem> doOtherStuff = null)
    {
        var l = Log.Fn<bool>(timer: true);
        if (!IsEnabled) return l.ReturnFalse("disabled");
        if (data == null) return l.ReturnFalse("null");
        if (data.IsError) return l.ReturnFalse("error");
        if (!data.CanCache) return l.ReturnFalse("can't cache");
        if (data == Existing?.Data) return l.ReturnFalse("not new");
        if (data.DependentApps.SafeNone()) return l.ReturnFalse("app not initialized");
        if (!UrlParams.CachingAllowed) return l.ReturnFalse("url params not allowed");

        // get dependent appStates
        var dependentAppsStates = data.DependentApps
            .Select(da => appReadersLazy.Value.Get(da.AppId))
            .ToList();
        l.A($"{nameof(dependentAppsStates)} count {dependentAppsStates.Count}");

        // when dependent apps have disabled caching, parent app should not cache also 
        if (!IsEnabledOnDependentApps(dependentAppsStates))
            return l.ReturnFalse("disabled in dependent app");

        // respect primary app (of site) as dependent app to ensure cache invalidation when primary app is changed
        var appState = AppState;
        if (appState == null)
            return l.ReturnFalse("no app");

        data.AppId = appState.AppId; // info for LightSpeedStats

        if (appState.ZoneId >= 0)
        {
            l.A("dependentAppsStates add");
            var primary = appsCatalog.Value.PrimaryAppIdentity(appState.ZoneId);
            dependentAppsStates.Add(appReadersLazy.Value.Get(primary));
        }

        l.A($"Found {data.DependentApps.Count} apps: " + string.Join(",", data.DependentApps.Select(da => da.AppId)));

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

        l.A($"LightSpeed Cache Key: {cacheKey}");

        return l.ReturnTrue($"added for {duration}s");
    }

    /// <summary>
    /// find if caching is enabled on all dependent apps
    /// </summary>
    private bool IsEnabledOnDependentApps(List<IAppReader> appStates)
    {
        var l = Log.Fn<bool>(timer: true);
        foreach (var appState in appStates.Where(appState => !GetLightSpeedConfig(appState).IsEnabled))
            return l.ReturnFalse($"Can't cache; caching disabled on dependent app {appState.AppId}");
        return l.ReturnTrue("ok");
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
        if ((_block as BlockFromModule)?.App is not EavApp app) return null;
        if (dependentApps.SafeNone()) return null;

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

    private (bool CachingAllowed, string Extension) UrlParams => _urlParams.Get(
        () => LightSpeedUrlParams.GetUrlParams(ViewConfig ?? AppConfig, _block.Context.Page.Parameters, Log)
        );
    private readonly GetOnce<(bool CachingAllowed, string Extension)> _urlParams = new();
    
    private string CurrentCulture => _currentCulture.Get(() => cmsContext.Value.Culture.CurrentCode);
    private readonly GetOnce<string> _currentCulture = new();


    private string CacheKey => _key.Get(() => Log.Func(() => OutputCacheManager.Id(_moduleId, _pageId, UserIdOrAnon, ViewKey, UrlParams.Extension, CurrentCulture)));
    private readonly GetOnce<string> _key = new();

    private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? null : _block.Context.User.Id);
    private readonly GetOnce<int?> _userId = new();

    // Note 2023-10-30 2dm changed the handling of the preview template and checks if it's set. In case caching is too aggressive this can be the problem. Remove early 2024
    private string ViewKey => _viewKey.Get(() => _block.Configuration?.PreviewViewEntity != null ? $"{_block.Configuration.AppId}:{_block.Configuration.View?.Id}" : null);
    private readonly GetOnce<string> _viewKey = new();

    public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
    private readonly GetOnce<OutputCacheItem> _existing = new();

    private OutputCacheItem ExistingGenerator()
    {
        var l = Log.Fn<OutputCacheItem>();
        try
        {
            if (AppState == null) return l.ReturnNull("no app");

            var result = IsEnabled ? OutCacheMan.Get(CacheKey) : null;
            if (result == null) return l.ReturnNull("not in cache");

            // compare cache time-stamps
            var dependentApp = result.Data?.DependentApps?.FirstOrDefault();
            if (dependentApp == null) return l.ReturnNull("no dep app");

            return l.Return(result, "found");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("error");
        }
    }


    public bool IsEnabled => _enabled.Get(IsEnabledGenerator);
    private readonly GetOnce<bool> _enabled = new();

    private bool IsEnabledGenerator()
    {
        var l = Log.Fn<bool>();
        // special - Oqtane seems to call this much earlier than Dnn, even on non-existing modules.
        // so on new modules this would fail and throw an error. So we'll just return false in this case.
        if (AppState == null)
            return l.ReturnFalse("disabled, no app");

        // Normal check.
        var feat = features.IsEnabled(LightSpeedOutputCache.NameId);
        if (!feat)
            return l.ReturnFalse("disabled, feature");

        if (!AppConfig.IsEnabled)
            return l.ReturnFalse("disabled, app not enabled");

        if (ViewConfig.IsEnabledNullable == false)
            return l.ReturnFalse("disabled at view explicit");

        if (!UrlParams.CachingAllowed)
            return l.ReturnFalse("disabled, url params not allowed");

        return l.ReturnTrue("enabled");
    }

    /// <summary>
    /// Lightspeed Configuration at App Level
    /// </summary>
    private LightSpeedDecorator AppConfig => _lsd.Get(() => GetLightSpeedConfig(AppState));
    private readonly GetOnce<LightSpeedDecorator> _lsd = new();

    /// <summary>
    /// Lightspeed Configuration at View Level
    /// </summary>
    private LightSpeedDecorator ViewConfig => _viewConfig.Get(() => _block.View?.Metadata.OfType(LightSpeedDecorator.TypeNameId).FirstOrDefault()?.NullOrGetWith(viewLs => new LightSpeedDecorator(viewLs)));
    private readonly GetOnce<LightSpeedDecorator> _viewConfig = new();

    private LightSpeedDecorator GetLightSpeedConfig(IAppReader appReader)
    {
        var l = Log.Fn<LightSpeedDecorator>();
        var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(appReader, Log);
        return l.Return(decoFromPiggyBack, $"has decorator: {decoFromPiggyBack.Entity != null}");
    }

    private OutputCacheManager OutCacheMan => outputCacheManager.Value;

}