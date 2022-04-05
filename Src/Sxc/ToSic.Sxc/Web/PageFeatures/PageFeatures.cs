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
        public IEnumerable<string> Activate(params string[] keys)
        {
            var realKeys = keys.Where(k => !string.IsNullOrWhiteSpace(k)).ToArray();
            FeatureKeys.AddRange(realKeys);
            return realKeys;
        }

        private List<PageFeatureFromSettings> FeaturesFromSettings { get; } = new List<PageFeatureFromSettings>();

        public void FeaturesFromSettingsAdd(PageFeatureFromSettings newFeature) => FeaturesFromSettings.Add(newFeature);

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
            var asIPageFeature = newFeatures.Cast<IPageFeature>().ToList();
            return wrapLog($"{asIPageFeature.Count}", asIPageFeature);
        }



        private List<string> FeatureKeys { get; } = new List<string>();
        

        public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log)
        {
            var wrapLog = log.Call<List<IPageFeature>>();
            log.Add("Try to get new specs from IPageService");
            //var features = FeatureKeys.ToList();
            //log.Add($"Got {features.Count} items");
            var unfolded = GetWithDependents(FeatureKeys.ToList(), log); // _pfm.GetWithDependents(features);
            //log.Add($"Got unfolded features {unfolded.Count}");
            FeatureKeys.Clear();
            return wrapLog("ok", unfolded);
        }

        /// <inheritdoc />
        public List<IPageFeature> GetWithDependents(List<string> features, ILog log)
        {
            var wrapLog = log.Call<List<IPageFeature>>();
            log.Add($"Got {features.Count} items");
            var unfolded = _pfm.GetWithDependents(features);
            log.Add($"Got unfolded features {unfolded.Count}");
            return wrapLog("ok", unfolded);
        }

    }
}
