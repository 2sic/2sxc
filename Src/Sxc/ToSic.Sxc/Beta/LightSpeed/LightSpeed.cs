using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Beta.LightSpeed
{
    public class LightSpeed: HasLog, IOutputCache
    {
        public LightSpeed(IFeaturesService features, AppResetMonitors appResetMonitors) : base(Constants.SxcLogName + ".Lights")
        {
            _features = features;
            _appResetMonitors = appResetMonitors;
        }

        ~LightSpeed() => AppState.AppStateChanged -= HandleAppStateChanged;

        private readonly IFeaturesService _features;
        private readonly AppResetMonitors _appResetMonitors;

        public IOutputCache Init(int moduleId, IBlock block)
        {
            var wrapLog = Log.Call<IOutputCache>($"mod: {moduleId}");
            _moduleId = moduleId;
            _block = block;
            AppState.AppStateChanged += HandleAppStateChanged;
            return wrapLog($"{IsEnabled}", this);
        }
        private int _moduleId;
        private IBlock _block;
        private AppState AppState => _block?.Context?.AppState;


        public bool Save(IRenderResult data)
        {
            var wrapLog = Log.Call<bool>();
            if (!IsEnabled) return wrapLog("disabled", false);
            if (data == null) return wrapLog("null", false);
            if (data.IsError) return wrapLog("error", false);
            if (!data.CanCache) return wrapLog("can't cache", false);
            if (data.DependentApps?.Any() != true) return wrapLog("app not initialized", false);
            if (data == Existing?.Data) return wrapLog("not new", false);
            Fresh.Data = data;
            var duration = Duration;
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return wrapLog($"not added as duration is {duration}", false);
            var cacheKey = Ocm.Add(CacheKey, Fresh, duration);
            Log.Add($"Cache Key: {cacheKey}");
            return wrapLog($"added for {duration}s", true);
        }

        private int Duration => _duration.Get(() =>
        {
            var user = _block.Context.User;
            if (user.IsSuperUser) return AppConfig.DurationSystemAdmin;
            if (user.IsAdmin) return AppConfig.DurationEditor;
            if (!user.IsAnonymous) return AppConfig.DurationUser;
            return AppConfig.Duration;
        });
        private readonly ValueGetOnce<int> _duration = new ValueGetOnce<int>();



        private string Suffix => _suffix.Get(GetSuffix);
        private readonly ValueGetOnce<string> _suffix = new ValueGetOnce<string>();
        private string GetSuffix()
        {
            if (!AppConfig.ByUrlParam) return null;
            var urlParams = _block.Context.Page.Parameters.ToString();
            if (string.IsNullOrWhiteSpace(urlParams)) return null;
            if (!AppConfig.UrlParamCaseSensitive) urlParams = urlParams.ToLowerInvariant();
            return urlParams;
        }


        private string CacheKey =>
            _key.Get(() => Log.Intercept(nameof(CacheKey), () => Ocm.Id(_moduleId, UserIdOrAnon, Suffix)));
        private readonly ValueGetOnce<string> _key = new ValueGetOnce<string>();


        private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? (int?)null : _block.Context.User.Id);
        private readonly ValueGetOnce<int?> _userId = new ValueGetOnce<int?>();


        public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
        private readonly ValueGetOnce<OutputCacheItem> _existing = new ValueGetOnce<OutputCacheItem>();
        private OutputCacheItem ExistingGenerator()
        {
            var wrapLog = Log.Call<OutputCacheItem>();
            if (AppState == null) return wrapLog("no app", null);

            var result = IsEnabled ? Ocm.Get(CacheKey) : null;
            if (result == null) return wrapLog("not in cache", null);

            // compare cache time-stamps
            var dependentApp = result.Data?.DependentApps?.FirstOrDefault();
            if (dependentApp == null) return wrapLog("no dep app", null);
            return AppState.CacheTimestamp > dependentApp.CacheTimestamp
                ? wrapLog("app changed", null)
                : wrapLog("found", result);
        }


        public OutputCacheItem Fresh => _fresh ?? (_fresh = new OutputCacheItem());
        private OutputCacheItem _fresh;


        public bool IsEnabled => _enabled.Get(IsEnabledGenerator);
        private readonly ValueGetOnce<bool> _enabled = new ValueGetOnce<bool>();
        private bool IsEnabledGenerator()
        {
            var wrapLog = Log.Call<bool>();
            var feat = _features.IsEnabled(FeaturesCatalog.LightSpeedOutputCache.NameId);
            if (!feat) return wrapLog("feature disabled", false);
            var ok = AppConfig.IsEnabled;
            return wrapLog($"app config: {ok}", ok);
        }




        public LightSpeedDecorator AppConfig => _lsd.Get(LightSpeedDecoratorGenerator);
        private readonly ValueGetOnce<LightSpeedDecorator> _lsd = new ValueGetOnce<LightSpeedDecorator>();
        private LightSpeedDecorator LightSpeedDecoratorGenerator()
        {
            var wrapLog = Log.Call<LightSpeedDecorator>();
            var decoEntityOrNull = AppState?.Metadata?.FirstOrDefaultOfType(LightSpeedDecorator.TypeName);
            var deco = new LightSpeedDecorator(decoEntityOrNull);
            return wrapLog($"{decoEntityOrNull != null}", deco);
        }


        private OutputCacheManager Ocm => _ocm ?? (_ocm = new OutputCacheManager(_appResetMonitors));
        private OutputCacheManager _ocm;


        public void HandleAppStateChanged(object sender, EventArgs e)
        {
            // flush a cache and dispose ChangeMonitor
            if (sender is AppState appState && _appResetMonitors.Monitors.TryRemove(appState.AppId /*e.AppId*/, out var appResetMonitorToDispose))
                appResetMonitorToDispose.Flush();
        }
    }
}
