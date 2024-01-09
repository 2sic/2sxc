using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Web.Internal.PageFeatures;

internal class PageFeaturesManager: IPageFeaturesManager
{
    /// <summary>
    /// Constructor - ATM we'll just add our known services here. 
    /// </summary>
    public PageFeaturesManager(PageFeaturesCatalog catalog) => _catalog = catalog;
    private readonly PageFeaturesCatalog _catalog;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, IPageFeature> Features => _catalog.Dictionary;

    public List<IPageFeature> GetWithDependents(List<string> keys)
    {
        if (keys == null || !keys.Any()) return new List<IPageFeature>();
        var collected = Get(keys);

        var added = collected;
            
        // Go max 5 levels deep
        for (var i = 0; i < 5; i++)
        {
            added = GetMissingDependencies(collected, added);
            if (!added.Any()) break;
            collected.AddRange(added);
        }
        return collected.Distinct().ToList();
    }

    private List<IPageFeature> GetMissingDependencies(IReadOnlyCollection<IPageFeature> target, IEnumerable<IPageFeature> toCheck)
    {
        // See what we still need
        var requiredKeys = toCheck.SelectMany(f => f.Needs).ToArray();
            
        // Skip those already in the target
        var missing = requiredKeys.Where(si =>
            !target.Any(c => c.NameId.Equals(si, StringComparison.InvariantCultureIgnoreCase))).ToArray();
        return Get(missing);
    }

    private List<IPageFeature> Get(IEnumerable<string> keys) => 
        Features.Values.Where(f => keys.Contains(f.NameId, StringComparer.InvariantCultureIgnoreCase)).ToList();
}