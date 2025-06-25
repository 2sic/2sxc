
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

[PrivateApi("Internal stuff, hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PageFeatures(IPageFeaturesManager pfm) : IPageFeatures
{
    /// <inheritdoc />
    public IEnumerable<string> Activate(params string[] keys)
    {
        var realKeys = keys.TrimmedAndWithoutEmpty();
        FeatureKeys.AddRange(realKeys);
        return realKeys;
    }

    private List<PageFeatureFromSettings> FeaturesFromSettings { get; } = [];

    public void FeaturesFromSettingsAdd(PageFeatureFromSettings newFeature)
        => FeaturesFromSettings.Add(newFeature);

    /// <summary>
    /// Get the manual features which were added - skip those which were previously already added
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    internal List<PageFeatureFromSettings> FeaturesFromSettingsGetNew(RenderSpecs specs, ILog log)
    {
        var l = log.Fn<List<PageFeatureFromSettings>>();
        // Filter out the ones which were already added in a previous round
        var newFeatures = FeaturesFromSettings
            // Put duplicates together
            .GroupBy(f => f.NameId)
            // Only process the groups which have none that were already processed
            .Where(g => g.All(f => !f.AlreadyProcessed )) // only keep the groups which only have false
            // Keep only one of the group to then process
            .Select(g => g.First())
            .ToList();

        // Mark the new ones as processed now, so they won't be processed in the future, except for Oqtane where we will keep it all
        if (!specs.IncludeAllAssetsInOqtane)
            newFeatures.ForEach(f => f.AlreadyProcessed = true);

        return l.Return(newFeatures, $"{newFeatures.Count}");
    }

    private List<string> FeatureKeys { get; } = [];


    public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log)
    {
        var l = log.Fn<List<IPageFeature>>();
        var unfolded = GetWithDependents([.. FeatureKeys], log);
        FeatureKeys.Clear();
        return l.Return(unfolded);
    }

    /// <inheritdoc />
    public List<IPageFeature> GetWithDependents(List<string> features, ILog log)
    {
        var l = log.Fn<List<IPageFeature>>($"Got {features.Count} items");
        var unfolded = pfm.GetWithDependents(features);
        return l.Return(unfolded, $"Got unfolded features {unfolded.Count}");
    }

}