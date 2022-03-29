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
        public LightSpeed(IFeaturesService features) : base(Constants.SxcLogName + ".Lights") 
            => _features = features;
        private readonly IFeaturesService _features;

        public IOutputCache Init(int moduleId, IBlock block)
        {
            var wrapLog = Log.Call<IOutputCache>($"mod: {moduleId}");
            _moduleId = moduleId;
            _block = block;
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
            if (data == Existing?.Data) return wrapLog("not new", false);
            Fresh.Data = data;
            var duration = GetDuration();
            // only add if we really have a duration; -1 is disabled, 0 is not set...
            if (duration <= 0)
                return wrapLog($"not added as duration is {duration}", false);
            var cacheKey = Ocm.Add(CacheKey, Fresh, duration);
            Log.Add($"Cache Key: {cacheKey}");
            return wrapLog($"added for {duration}s", true);
        }

        private int GetDuration()
        {
            var user = _block.Context.User;
            if (user.IsSuperUser) return AppConfig.DurationSystemAdmin;
            if (user.IsAdmin) return AppConfig.DurationEditor;
            if (!user.IsAnonymous) return AppConfig.DurationUser;
            return AppConfig.Duration;
        }



        private string Suffix => _suffix.Get(GetSuffix);
        private readonly PropertyToRetrieveOnce<string> _suffix = new PropertyToRetrieveOnce<string>();
        private string GetSuffix() => !AppConfig.ByUrlParam ? null : _block.Context.Page.Parameters.ToString();


        private string CacheKey =>
            _key.Get(() => Log.Intercept(nameof(CacheKey), () => Ocm.Id(_moduleId, UserIdOrAnon, Suffix)));
        private readonly PropertyToRetrieveOnce<string> _key = new PropertyToRetrieveOnce<string>();


        private int? UserIdOrAnon => _userId.Get(() => _block.Context.User.IsAnonymous ? (int?)null : _block.Context.User.Id);
        private readonly PropertyToRetrieveOnce<int?> _userId = new PropertyToRetrieveOnce<int?>();


        //public bool IsInCache => Existing != null;


        public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
        private readonly PropertyToRetrieveOnce<OutputCacheItem> _existing = new PropertyToRetrieveOnce<OutputCacheItem>();
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
        private readonly PropertyToRetrieveOnce<bool> _enabled = new PropertyToRetrieveOnce<bool>();
        private bool IsEnabledGenerator()
        {
            var wrapLog = Log.Call<bool>();
            var feat = _features.IsEnabled(FeaturesCatalog.LightSpeedOutputCache.NameId);
            if (!feat) return wrapLog("feature disabled", false);
            var ok = AppConfig.IsEnabled;
            return wrapLog($"app config: {ok}", ok);
        }




        public LightSpeedDecorator AppConfig => _lsd.Get(LightSpeedDecoratorGenerator);
        private readonly PropertyToRetrieveOnce<LightSpeedDecorator> _lsd = new PropertyToRetrieveOnce<LightSpeedDecorator>();
        private LightSpeedDecorator LightSpeedDecoratorGenerator()
        {
            var wrapLog = Log.Call<LightSpeedDecorator>();
            var decoEntityOrNull = AppState?.Metadata?.FirstOrDefaultOfType(LightSpeedDecorator.TypeName);
            var deco = new LightSpeedDecorator(decoEntityOrNull);
            return wrapLog($"{decoEntityOrNull != null}", deco);
        }


        private OutputCacheManager Ocm => _ocm ?? (_ocm = new OutputCacheManager());
        private OutputCacheManager _ocm;

    }
}
