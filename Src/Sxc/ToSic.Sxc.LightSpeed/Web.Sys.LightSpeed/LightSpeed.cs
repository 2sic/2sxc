using System.Diagnostics.CodeAnalysis;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Sys.Configuration.SxcFeatures;

namespace ToSic.Sxc.Web.Sys.LightSpeed;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class LightSpeed(
    ISysFeaturesService features,
    LazySvc<ISite> site,
    LazySvc<OutputCacheManager> outputCacheManager
) : ServiceBase(SxcLogName + ".Lights", connect: [features, outputCacheManager]), IOutputCache
{
    [field: AllowNull, MaybeNull]
    private LightSpeedConfigHelper LsConfigHelper => field ??= new(Log);

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
    private IBlock? _block;
    private IAppReader? AppReaderOrNull => field ??= _block?.Context?.AppReaderOrNull;

    public bool Save(IRenderResult data) => AddToLightSpeed(data);

    // #RemovedV20 #OldDnnAutoJQuery
    //#if NETFRAMEWORK
    //    public bool Save(IRenderResult data, bool enforcePre1025)
    //        => AddToLightSpeed(data, cacheData => cacheData.EnforcePre1025 = enforcePre1025);
    //#endif

    // #RemovedV20 #OldDnnAutoJQuery
    public bool AddToLightSpeed(IRenderResult? data/*, Action<OutputCacheItem>? doOtherStuff = null*/)
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
        try
        {
            l.A($"{nameof(data.DependentApps)} count {data.DependentApps.Count}");

            // when dependent apps have disabled caching, parent app should not cache also 
            if (!IsEnabledOnDependentApps(data.DependentApps))
                return l.ReturnFalse("disabled in dependent app");

            // respect primary app (of site) as dependent app to ensure cache invalidation when primary app is changed
            if (AppReaderOrNull == null)
                return l.ReturnFalse("no app");

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
            // #RemovedV20 #OldDnnAutoJQuery
            //doOtherStuff?.Invoke(cacheItem);

            var duration = Duration;
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return l.ReturnFalse($"not added as duration is {duration}");

            var appPathsToMonitor = features.IsEnabled(LightSpeedOutputCacheAppFileChanges.NameId)
                ? data.DependentApps.SelectMany(da => da.PathsToMonitor).ToList()
                : null;
            l.A($"{nameof(appPathsToMonitor)} done");

            // add to cache and log
            string? cacheKey = null;
            l.Do(message: "outputCacheManager add", timer: true,
                action: () => cacheKey = OutCacheMan.Add(
                    CacheKey,
                    cacheItem,
                    duration,
                    data.DependentApps.SelectMany(r => r.CacheKeys).ToList(),
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
    private bool IsEnabledOnDependentApps(List<IDependentApp> dependentApps)
    {
        var l = Log.Fn<bool>(timer: true);
        var appWhereNotEnabled = dependentApps
            .FirstOrDefault(dependentApp => !dependentApp.IsEnabled);

        return appWhereNotEnabled != null
            ? l.ReturnFalse($"Can't cache; caching disabled on dependent app {appWhereNotEnabled.AppId}")
            : l.ReturnTrue("ok");
    }

    private int Duration => _duration ??= _block!.Context.User switch
    {
        { IsSystemAdmin: true } => AppConfig.DurationSystemAdmin,
        { IsSiteAdmin: true } => AppConfig.DurationEditors,
        { IsAnonymous: false } => AppConfig.DurationUsers,
        _ => AppConfig.Duration
    };
    private int? _duration;

    private (bool CachingAllowed, string Extension) UrlParams => _urlParams.Get(() =>
        LightSpeedUrlParams.GetUrlParams(ViewConfigOrNull ?? AppConfig, _block?.Context.Page.Parameters, Log)
    );
    private readonly GetOnce<(bool CachingAllowed, string Extension)> _urlParams = new();
    
    private string CurrentCulture => _currentCulture.Get(() => site.Value.SafeCurrentCultureCode())!;
    private readonly GetOnce<string> _currentCulture = new();


    private string CacheKey => _key.Get(() => Log.Quick(()
        => OutputCacheKeys.ModuleKey(_pageId, _moduleId, UserIdOrAnon, ViewKey, UrlParams.Extension, CurrentCulture)
    ))!;
    private readonly GetOnce<string> _key = new();

    private int? UserIdOrAnon => _userId.Get(() => _block?.Context.User.IsAnonymous == false ? _block.Context.User.Id : null);
    private readonly GetOnce<int?> _userId = new();

    // Note 2023-10-30 2dm changed the handling of the preview template and checks if it's set. In case caching is too aggressive this can be the problem. Remove early 2024
    private string? ViewKey => _viewKey.Get(() => _block is { ConfigurationIsReady: true, Configuration.PreviewViewEntity: not null }
        ? $"{_block.Configuration.AppId}:{_block.Configuration.View?.Id}"
        : null
    );
    private readonly GetOnce<string?> _viewKey = new();

    public OutputCacheItem? Existing => _existing.Get(GetExisting);
    private readonly GetOnce<OutputCacheItem?> _existing = new();

    private OutputCacheItem? GetExisting()
    {
        var l = Log.Fn<OutputCacheItem>();
        try
        {
            // If App not known, it can't have a cache - exit early
            if (AppReaderOrNull == null)
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
            var dependentApp = result.Data.DependentApps?.FirstOrDefault();
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


    public bool IsEnabled => _enabled.Get(GetLightSpeedIsEnabled);
    private readonly GetOnce<bool> _enabled = new();

    private bool GetLightSpeedIsEnabled()
    {
        var l = Log.Fn<bool>();

        //// No real app yet, probably module was just added
        //if (_block.App == null)
        //    return false;

        // This is called from outside, so we need to catch all exceptions as it should never break in production
        try
        {
            // special - Oqtane seems to call this much earlier than Dnn, even on non-existing modules.
            // so on new modules this would fail and throw an error. So we'll just return false in this case.
            if (AppReaderOrNull == null)
                return l.ReturnFalse("disabled, no app");

            // Normal check.
            l.A("before features check");
            var feat = features.IsEnabled(LightSpeedOutputCache.NameId);
            l.A($"features: {feat}");
            if (!feat)
                return l.ReturnFalse("disabled, feature");

            var appMaybeEnabled = AppConfig.IsEnabledNullable;
            l.A("before app config deny check");
            if (appMaybeEnabled == false)
                return l.ReturnFalse("disabled, app explicit");

            var viewMaybeEnabled = ViewConfigOrNull?.IsEnabledNullable;
            l.A("before view config deny check");
            if (viewMaybeEnabled == false)
                return l.ReturnFalse("disabled, view explicit");

            l.A("before view/app is enabled check - either should be true");
            if (appMaybeEnabled != true && viewMaybeEnabled != true)
                return l.ReturnFalse($"disabled, neither app '{appMaybeEnabled}' nor view '{viewMaybeEnabled}' enabled");

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
    private LightSpeedDecorator AppConfig => _lsd.Get(() => LsConfigHelper.GetLightSpeedConfigOfApp(AppReaderOrNull))!;
    private readonly GetOnce<LightSpeedDecorator> _lsd = new();

    /// <summary>
    /// Lightspeed Configuration at View Level
    /// </summary>
    private LightSpeedDecorator? ViewConfigOrNull => _viewConfig.Get(() => LsConfigHelper.ViewConfigOrNull(_block));
    private readonly GetOnce<LightSpeedDecorator?> _viewConfig = new();

    private OutputCacheManager OutCacheMan => outputCacheManager.Value;

}