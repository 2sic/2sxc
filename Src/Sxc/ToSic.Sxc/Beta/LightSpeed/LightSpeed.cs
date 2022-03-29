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
        {
            _features = features;
        }
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

        public bool Save(IRenderResult data)
        {
            var wrapLog = Log.Call<bool>();
            if (!IsEnabled) return wrapLog("disabled", false);
            if (data == null) return wrapLog("null", false);
            if (data == Existing?.Data) return wrapLog("not new", false);
            Fresh.Data = data;
            Ocm.Add(_moduleId, Fresh, AppConfig.Duration);
            return wrapLog($"added for {AppConfig.Duration} seconds", true);
        }

        public bool IsInCache => Existing != null;

        public OutputCacheItem Existing => _existing.Get(ExistingGenerator);
        private readonly PropertyToRetrieveOnce<OutputCacheItem> _existing = new PropertyToRetrieveOnce<OutputCacheItem>();
        private OutputCacheItem ExistingGenerator()
        {
            var wrapLog = Log.Call<OutputCacheItem>();
            if (AppState == null) return wrapLog("no app", null);

            var result = IsEnabled ? Ocm.Get(_moduleId) : null;
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
            
            // 1. check feature (fastest)
            var feat = _features.IsEnabled(FeaturesCatalog.LightSpeedOutputCache.NameId);
            if (!feat) return wrapLog("feature disabled", false);

            // 2. Check App activated
            //var decoEntity = AppState?.Metadata?.FirstOrDefaultOfType(LightSpeedDecorator.TypeName);
            //if (decoEntity == null) return wrapLog("no app config", false);

            //var deco = new LightSpeedDecorator(decoEntity);
            var ok = AppConfig.IsEnabled;// deco.IsEnabled;

            return wrapLog($"app config: {ok}", ok);
        }
        private AppState AppState => _block?.Context?.AppState;

        public LightSpeedDecorator AppConfig => _lsd.Get(LightSpeedDecoratorGenerator);
        private readonly PropertyToRetrieveOnce<LightSpeedDecorator> _lsd = new PropertyToRetrieveOnce<LightSpeedDecorator>();
        private LightSpeedDecorator LightSpeedDecoratorGenerator()
        {
            var wrapLog = Log.Call<LightSpeedDecorator>();
            var decoEntityOrNull = AppState?.Metadata?.FirstOrDefaultOfType(LightSpeedDecorator.TypeName);
            //if (decoEntity == null) return wrapLog("no app config", null);

            var deco = new LightSpeedDecorator(decoEntityOrNull);
            return wrapLog($"{decoEntityOrNull != null}", deco);
        }



        private OutputCacheManager Ocm => _ocm ?? (_ocm = new OutputCacheManager());
        private OutputCacheManager _ocm;

    }
}
