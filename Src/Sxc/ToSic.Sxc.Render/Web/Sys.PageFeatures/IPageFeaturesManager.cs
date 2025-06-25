using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Web.Sys.PageFeatures;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPageFeaturesManager
{
    IReadOnlyDictionary<string, IPageFeature> Features { get; }
        
    List<IPageFeature> GetWithDependents(List<string> keys);
}