using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Hide implementation")]
    public class PageFeatures: IPageFeatures
    {
        private readonly IPageFeaturesManager _pfm;
        
        public PageFeatures(IPageFeaturesManager pfm)
        {
            _pfm = pfm;
        }

        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            var realKeys = keys.Where(k => !string.IsNullOrWhiteSpace(k));
            ActiveKeys.AddRange(realKeys);
            
        }

        private List<IPageFeature> ManualFeatures { get; } = new List<IPageFeature>();

        public void ManualFeatureAdd(IPageFeature newFeature) => ManualFeatures.Add(newFeature);

        public List<IPageFeature> ManualFeaturesGetNew()
        {
            // Filter out the ones which were already added in a previous round
            var newFeatures = ManualFeatures
                .GroupBy(f => f.Key)
                .Where(g => g.All(f => !f.AlreadyProcessed)) // only keep the groups which only have false
                .Select(g => g.First())
                .ToList();

            // Mark the new ones as processed now, so they won't be processed in future
            newFeatures.ForEach(f => f.AlreadyProcessed = true);
            return newFeatures;
        }










        private List<string> ActiveKeys { get; } = new List<string>();
        
        private List<string> GetKeysAndFlush()
        {
            var keys = ActiveKeys.ToArray().ToList();
            ActiveKeys.Clear();
            return keys;
        }

        public List<IPageFeature> GetWithDependentsAndFlush(ILog log)
        {
            // if (_features != null) return _features;
            var wrapLog = log.Call<List<IPageFeature>>();
            log.Add("Try to get new specs from IPageService");
            var features = GetKeysAndFlush();
            log.Add($"Got {features.Count} items");
            var unfolded = _pfm.GetWithDependents(features);
            log.Add($"Got unfolded features {unfolded.Count}");
            // _features = unfolded;
            return wrapLog("ok", unfolded);
        }

    }
}
