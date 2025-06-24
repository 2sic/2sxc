namespace ToSic.Sxc.Web.Internal.PageFeatures;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IPageFeaturesManager
{
    IReadOnlyDictionary<string, IPageFeature> Features { get; }
        
    List<IPageFeature> GetWithDependents(List<string> keys);
}