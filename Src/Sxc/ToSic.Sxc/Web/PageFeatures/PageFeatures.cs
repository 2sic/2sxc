using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Internal stuff, hide implementation")]
    public class PageFeatures: IPageFeatures
    {
        public PageFeatures(IPageFeaturesManager pfm) => _pfm = pfm;
        private readonly IPageFeaturesManager _pfm;

        /// <inheritdoc />
        public void Activate(params string[] keys)
        {
            var realKeys = keys.Where(k => !string.IsNullOrWhiteSpace(k));
            FeatureKeys.AddRange(realKeys);
        }

        private List<IPageFeature> FeaturesFromSettings { get; } = new List<IPageFeature>();

        public void FeaturesFromSettingsAdd(IPageFeature newFeature) => FeaturesFromSettings.Add(newFeature);

        public List<IPageFeature> FeaturesFromSettingsGetNew(ILog log)
        {
            var wrapLog = log.Call<List<IPageFeature>>();
            // Filter out the ones which were already added in a previous round
            var newFeatures = FeaturesFromSettings
                // Put duplicates together
                .GroupBy(f => f.Key)
                // Only process the groups which have none that were already processed
                .Where(g => g.All(f => !f.AlreadyProcessed)) // only keep the groups which only have false
                // Keep only one of the group to then process
                .Select(g => g.First())
                .ToList();

            // Mark the new ones as processed now, so they won't be processed in future
            newFeatures.ForEach(f => f.AlreadyProcessed = true);
            return wrapLog($"{newFeatures.Count}", newFeatures);
        }



        private List<string> FeatureKeys { get; } = new List<string>();
        
        private List<string> GetFeatureKeysAndFlush()
        {
            var keys = FeatureKeys.ToList();
            FeatureKeys.Clear();
            return keys;
        }

        public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log)
        {
            var wrapLog = log.Call<List<IPageFeature>>();
            log.Add("Try to get new specs from IPageService");
            var features = GetFeatureKeysAndFlush();
            log.Add($"Got {features.Count} items");
            var unfolded = _pfm.GetWithDependents(features);
            log.Add($"Got unfolded features {unfolded.Count}");
            return wrapLog("ok", unfolded);
        }

    }
}
