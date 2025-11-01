using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Web.Sys.PageFeatures;

[PrivateApi("Internal stuff, hide implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class PageFeatures(IPageFeaturesManager pfm) : IPageFeatures
{
    /// <inheritdoc />
    public IEnumerable<string> Activate(string[] keys)
    {
        var realKeys = keys.TrimmedAndWithoutEmpty();
        FeatureKeysRecent.AddRange(realKeys);
        return realKeys;
    }

    /// <summary>
    /// Must be a real List, because it will be modified.
    /// </summary>
    private List<PageFeatureFromSettings> FeaturesFromSettings { get; } = [];

    private List<PageFeatureFromSettings> FeaturesFromSettingsProcessed { get; } = [];

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
            // 2025-08-17 2dm changed detection of already-processed because it resulted in changing the original object which caused problems in caching
            .Where(f => !FeaturesFromSettingsProcessed.Any(p => ReferenceEquals(p, f))) // only keep the groups which only have false
            .ToList();

        // Mark the new ones as processed now, so they won't be processed in the future, except for Oqtane where we will keep it all
        if (!specs.IncludeAllAssetsInOqtane)
            FeaturesFromSettingsProcessed.AddRange(newFeatures);

        newFeatures = newFeatures
            // Put duplicates together
            .GroupBy(f => f.NameId)
            // Only process the groups which have none that were already processed (old before 2025-08-17)
            //.Where(g => g.All(f => !f.AlreadyProcessed )) // only keep the groups which only have false
            // Keep only one of the group to then process
            .Select(g => g.First())
            .ToList();

        return l.Return(newFeatures, $"{newFeatures.Count}");
    }

    /// <summary>
    /// The recently added feature keys.
    /// Will be flushed regularly, as each module will make sure that it's added to the page.
    /// </summary>
    private List<string> FeatureKeysRecent { get; } = [];



    public List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log)
    {
        var l = log.Fn<List<IPageFeature>>();
        var unfolded = GetWithDependents([.. FeatureKeysRecent], log);
        FeatureKeysRecent.Clear();
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