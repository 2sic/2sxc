namespace ToSic.Sxc.Web.Internal.PageFeatures;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPageFeaturesManager
{
    IReadOnlyDictionary<string, IPageFeature> Features { get; }
        
    List<IPageFeature> GetWithDependents(List<string> keys);
}