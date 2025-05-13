using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

/// <summary>
/// Constructor - ATM we'll just add our known services here. 
/// </summary>
internal class PageFeaturesManager(PageFeaturesCatalog catalog) : IPageFeaturesManager
{
    /// <inheritdoc />
    public IReadOnlyDictionary<string, IPageFeature> Features => catalog.Dictionary;

    public List<IPageFeature> GetWithDependents(List<string> keys)
    {
        if (keys == null || !keys.Any()) return [];
        var collected = Get(keys).Internal;

        var added = collected;
            
        // Go max 5 levels deep
        for (var i = 0; i < 5; i++)
        {
            added = GetMissingDependencies(collected, added).Internal;
            if (!added.Any()) break;
            collected.AddRange(added);
        }
        return collected.Distinct().ToList();
    }

    private (List<IPageFeature> Internal, List<IPageFeature> External) GetMissingDependencies(IReadOnlyCollection<IPageFeature> target, IEnumerable<IPageFeature> toCheck)
    {
        // See what we still need
        var requiredKeys = toCheck
            .SelectMany(f => f.Needs)
            .ToArray();
            
        // Skip those already in the target
        var missing = requiredKeys
            .Where(si => !target.Any(c => c.NameId.EqualsInsensitive(si)))
            .ToArray();
        return Get(missing);
    }

    private (List<IPageFeature> Internal, List<IPageFeature> External) Get(IEnumerable<string> keys)
    {
        //return Features.Values
        //    .Where(f => keys.Contains(f.NameId, StringComparer.InvariantCultureIgnoreCase))
        //    .ToList();

        var split = Features.Values
            .GroupBy(f => keys.Contains(f.NameId, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        var result =
        (
            split.FirstOrDefault(g => g.Key)?.ToList() ?? [],
            split.FirstOrDefault(g => !g.Key)?.ToList() ?? []
        );
        return result;
    }
}