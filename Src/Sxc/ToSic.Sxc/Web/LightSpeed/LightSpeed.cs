using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Caching;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Web.LightSpeed
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class LightSpeed : ServiceBase, IOutputCache
    {

        public LightSpeed(IFeaturesInternal features, LazySvc<IAppStates> appStatesLazy, LazySvc<AppPaths> appPathsLazy, LightSpeedStats lightSpeedStats, LazySvc<ICmsContext> cmsContext) : base(Constants.SxcLogName + ".Lights")
        {
            ConnectServices(
                LightSpeedStats = lightSpeedStats,
                _features = features,
                _appStatesLazy = appStatesLazy,
                _appPathsLazy = appPathsLazy,
                _cmsContext = cmsContext
            );
        }
        public LightSpeedStats LightSpeedStats { get; }
        private readonly IFeaturesInternal _features;
        private readonly LazySvc<IAppStates> _appStatesLazy;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<ICmsContext> _cmsContext;

        public IOutputCache Init(int moduleId, int pageId, IBlock block)
        {
            var wrapLog = Log.Fn<IOutputCache>($"mod: {moduleId}");
            _moduleId = moduleId;
            _pageId = pageId;
            _block = block;
            return wrapLog.Return(this, $"{IsEnabled}");
        }
        private int _moduleId;
        private int _pageId;
        private IBlock _block;
        private AppState AppState => _block?.Context?.AppState;
        private IAppStates AppStates => _appStatesLazy.Value;

        public bool Save(IRenderResult data)
        {
            var l = Log.Fn<bool>(timer: true);
            if (!IsEnabled) return l.ReturnFalse("disabled");
            if (data == null) return l.ReturnFalse("null");
            if (data.IsError) return l.ReturnFalse("error");
            if (!data.CanCache) return l.ReturnFalse("can't cache");
            if (data == Existing?.Data) return l.ReturnFalse("not new");
            if (data.DependentApps.SafeNone()) return l.ReturnFalse("app not initialized");

            // get dependent appStates
            List<AppState> dependentAppsStates = null;
            l.Do(message: "dependentAppsStates", timer: true, 
                action: () => dependentAppsStates = data.DependentApps.Select(da => AppStates.Get(da.AppId)).ToList());

            // when dependent apps have disabled caching, parent app should not cache also 
            if (!IsEnabledOnDependentApps(dependentAppsStates)) return l.ReturnFalse("disabled in dependent app");

            // respect primary app (of site) as dependent app to ensure cache invalidation when primary app is changed
            var appState = AppState;
            if (appState?.ZoneId != null)
                l.Do(message: "dependentAppsStates add", timer: true,
                    action: () => dependentAppsStates.Add(AppStates.Get(AppStates.IdentityOfPrimary(appState.ZoneId))));

            l.A($"Found {data.DependentApps.Count} apps: " + string.Join(",", data.DependentApps.Select(da => da.AppId)));
            Fresh.Data = data;
            var duration = Duration;
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return l.ReturnFalse($"not added as duration is {duration}");
            
            IList<string> appPathsToMonitor = null;
            l.Do(message: "appPathsToMonitor", timer: true, action: () => 
                appPathsToMonitor = _features.IsEnabled(LightSpeedOutputCacheAppFileChanges.NameId)
                ? _appPaths.Get(() => AppPaths(dependentAppsStates))
                : null);

            string cacheKey = null;
            l.Do(message: "outputCacheManager add", timer: true, action: () =>
                cacheKey = OutCacheMan.Add(CacheKey, Fresh, duration, _features, dependentAppsStates, appPathsToMonitor,
                (x) => LightSpeedStats.Remove(appState.AppId, data.Size)));

            l.A($"LightSpeed Cache Key: {cacheKey}");

            if (cacheKey != "error")
                l.Do(message: "LightSpeedStats", timer: true,
                    action: () => LightSpeedStats.Add(appState.AppId, data.Size));

            return l.ReturnTrue($"added for {duration}s");
        }

        /// <summary>
        /// find if caching is enabled on all dependent apps
        /// </summary>
        private bool IsEnabledOnDependentApps(List<AppState> appStates)
        {
            var l = Log.Fn<bool>(timer: true);
            foreach (var appState in appStates)
            {
                var appConfig = LightSpeedDecorator.GetFromAppStatePiggyBack(appState, Log);
                if (appConfig.IsEnabled == false)
                    return l.ReturnFalse($"Can't cache; caching disabled on dependent app {appState.AppId}");
            }
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
        private IList<string> AppPaths(List<AppState> appStates)
        {
            if (!((_block as BlockFromModule)?.App is App app)) return null;
            if (appStates.SafeNone()) return null;

            var paths = new List<string>();
            foreach (var appState in appStates)
            {
                var appPaths = _appPathsLazy.Value.Init(app.Site, appState);
                if (Directory.Exists(appPaths.PhysicalPath)) paths.Add(appPaths.PhysicalPath);
                if (Directory.Exists(appPaths.PhysicalPathShared)) paths.Add(appPaths.PhysicalPathShared);
            }

            return paths;
        }
        private readonly GetOnce<IList<string>> _appPaths = new GetOnce<IList<string>>();

        private int Duration => _duration.Get(() =>
        {
            var user = _block.Context.User;
            if (user.IsSystemAdmin) return AppConfig.DurationSystemAdmin;
            if (user.IsSiteAdmin) return AppConfig.DurationEditor;
            if (!user.IsAnonymous) return AppConfig.DurationUser;
            return AppConfig.Duration;
        });
        private readonly GetOnce<int> _duration = new GetOnce<int>();

        private string Suffix => _suffix.Get(GetSuffix);
        private readonly GetOnce<string> _suffix = new GetOnce<string>();

        private string GetSuffix()
        {
            if (!AppConfig.ByUrlParam) return null;
            var urlParams = _block.Context.Page.Parameters.ToString();
            if (string.IsNullOrWhiteSpace(urlParams)) return null;
            if (!AppConfig.UrlParamCaseSensitive) urlParams = urlParams.ToLowerInvariant();
            return urlParams;
        }

        private string CurrentCulture => _currentCulture.Get(() => _cmsContext.Value.Culture.CurrentCode);
        private readonly GetOnce<string> _currentCulture = new GetOnce<string>();


        private string CacheKey => _key.Get(() => Log.Func(() => OutCacheMan.Id(_moduleId, _pageId, UserIdOrAnon, ViewKey, Suffix, CurrentCulture)));
        private readonly GetOnce<string> _key = new GetOnce<string>();

        private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? (int?)null : _block.Context.User.Id);
        private readonly GetOnce<int?> _userId = new GetOnce<int?>();

        // Note 2023-10-30 2dm changed the handling of the preview template and checks if it's set. In case caching is too aggressive this can be the problem. Remove early 2024
        private string ViewKey => _viewKey.Get(() => _block.Configuration?.PreviewTemplate != null ? $"{_block.Configuration.AppId}:{_block.Configuration.View?.Id}" : null);
        private readonly GetOnce<string> _viewKey = new GetOnce<string>();

        public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
        private readonly GetOnce<OutputCacheItem> _existing = new GetOnce<OutputCacheItem>();

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

        public OutputCacheItem Fresh => _fresh ?? (_fresh = new OutputCacheItem());
        private OutputCacheItem _fresh;


        public bool IsEnabled => _enabled.Get(IsEnabledGenerator);
        private readonly GetOnce<bool> _enabled = new GetOnce<bool>();

        private bool IsEnabledGenerator()
        {
            var l = Log.Fn<bool>();
            var feat = _features.IsEnabled(LightSpeedOutputCache.NameId);
            if (!feat) return l.ReturnFalse("feature disabled");
            var ok = AppConfig.IsEnabled;
            return l.Return(ok, $"app config: {ok}");
        }

        internal LightSpeedDecorator AppConfig => _lsd.Get(AppConfigGenerator);
        private readonly GetOnce<LightSpeedDecorator> _lsd = new GetOnce<LightSpeedDecorator>();

        private LightSpeedDecorator AppConfigGenerator()
        {
            var l = Log.Fn<LightSpeedDecorator>();
            var decoFromPiggyBack = LightSpeedDecorator.GetFromAppStatePiggyBack(AppState, Log);
            return l.Return(decoFromPiggyBack, $"{decoFromPiggyBack.Entity != null}");
        }


        private OutputCacheManager OutCacheMan
        {
            get
            {
                if (_ocm != null) return _ocm;
                ConnectServices(_ocm = new OutputCacheManager());
                return _ocm;
            }
        }
        private OutputCacheManager _ocm;
    }
}
