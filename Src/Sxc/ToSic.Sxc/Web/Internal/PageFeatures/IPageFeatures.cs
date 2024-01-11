namespace ToSic.Sxc.Web.Internal.PageFeatures;

/// <summary>
/// Part of the <see cref="ToSic.Sxc.Services.IPageService"/> to activate features on the page.
/// </summary>
[PrivateApi("should never be public, could be confused with the IPageService")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPageFeatures
{
    IEnumerable<string> Activate(params string[] keys);
        
    /// <summary>
    /// Get a list of all features incl. dependent features for adding to the page
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    List<IPageFeature> GetFeaturesWithDependentsAndFlush(ILog log);


    /// <summary>
    /// Internal helper to expand a set of keys to the features.
    /// It must be callable separately, because we also need it to expand features which are from another source. 
    /// </summary>
    /// <param name="features"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    List<IPageFeature> GetWithDependents(List<string> features, ILog log);

    /// <summary>
    /// Add a manual feature (having custom HTML)
    /// </summary>
    /// <param name="newFeature"></param>
    void FeaturesFromSettingsAdd(PageFeatureFromSettings newFeature);

    /// <summary>
    /// Get the manual features which were added - skip those which were previously already added
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    List<PageFeatureFromSettings> FeaturesFromSettingsGetNew(ILog log);

}