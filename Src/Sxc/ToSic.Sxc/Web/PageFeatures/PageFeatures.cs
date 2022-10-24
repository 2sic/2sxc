using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;

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

        public List<PageFeatureFromSettings> FeaturesFromSettingsGetNew(ILog log)
        {
            var wrapLog = log.Fn<List<PageFeatureFromSettings>>();
            // Filter out the ones which were already added in a previous round
            var newFeatures = FeaturesFromSettings
                // Put duplicates together
                .GroupBy(f => f.NameId)
                // Only process the groups which have none that were already processed
                .Where(g => g.All(f => !f.AlreadyProcessed)) // only keep the groups which only have false
                // Keep only one of the group to then process
                .Select(g => g.First())
                .ToList();

            // Mark the new ones as processed now, so they won't be processed in future
            newFeatures.ForEach(f => f.AlreadyProcessed = true);
            var asIPageFeature = newFeatures;
            return wrapLog.Return(asIPageFeature, $"{asIPageFeature.Count}");
        }



        private List<string> FeatureKeys { get; } = new List<string>();
        

        public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log)
        {
            var wrapLog = log.Fn<List<IPageFeature>>();
            log.A("Try to get new specs from IPageService");
            //var features = FeatureKeys.ToList();
            //log.Add($"Got {features.Count} items");
            var unfolded = GetWithDependents(FeatureKeys.ToList(), log);
            //log.Add($"Got unfolded features {unfolded.Count}");
            FeatureKeys.Clear();
            return wrapLog.ReturnAsOk(unfolded);
        }

        /// <inheritdoc />
        public List<IPageFeature> GetWithDependents(List<string> features, ILog log)
        {
            var wrapLog = log.Fn<List<IPageFeature>>();
            log.A($"Got {features.Count} items");
            var unfolded = _pfm.GetWithDependents(features);
            log.A($"Got unfolded features {unfolded.Count}");
            return wrapLog.ReturnAsOk(unfolded);
        }

    }
}
