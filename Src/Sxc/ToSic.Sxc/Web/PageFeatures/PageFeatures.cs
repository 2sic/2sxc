using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Internal stuff, hide implementation")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class PageFeatures: IPageFeatures
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

        private List<PageFeatureFromSettings> FeaturesFromSettings { get; } = new();

        public void FeaturesFromSettingsAdd(PageFeatureFromSettings newFeature) => FeaturesFromSettings.Add(newFeature);

        public List<PageFeatureFromSettings> FeaturesFromSettingsGetNew(ILog log) => log.Func(() =>
        {
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
            return (asIPageFeature, $"{asIPageFeature.Count}");
        });



        private List<string> FeatureKeys { get; } = new();


        public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log) => log.Func(() =>
        {
            var unfolded = GetWithDependents(FeatureKeys.ToList(), log);
            FeatureKeys.Clear();
            return unfolded;
        });

        /// <inheritdoc />
        public List<IPageFeature> GetWithDependents(List<string> features, ILog log) => log.Func($"Got {features.Count} items", () =>
        {
            var unfolded = _pfm.GetWithDependents(features);
            return (unfolded, $"Got unfolded features {unfolded.Count}");
        });

    }
}
