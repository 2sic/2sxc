using System.Collections.Generic;

namespace ToSic.Sxc.Web.PageFeatures
{
    public interface IPageFeaturesManager
    {
        IReadOnlyDictionary<string, IPageFeature> Features { get; }
        
        List<IPageFeature> GetWithDependents(List<string> keys);
    }
}
