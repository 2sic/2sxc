using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web.PageFeatures;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Part of the <see cref="ToSic.Sxc.Services.IPageService"/> to activate features on the page.
    /// </summary>
    [PrivateApi("not public ATM")]
    public interface IPageFeatures
    {
        //IPageFeatures Init(IPageService pageService);
        
        void Activate(params string[] keys);
        
        //List<string> ActiveKeys { get; }

        //List<string> GetKeysAndFlush();

        /// <summary>
        /// Get a list of all features incl. dependent features for adding to the page
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        List<IPageFeature> GetWithDependentsAndFlush(ILog log);

        /// <summary>
        /// Add a manual feature (having custom HTML)
        /// </summary>
        /// <param name="newFeature"></param>
        void ManualFeatureAdd(IPageFeature newFeature);

        /// <summary>
        /// Get the manual features which were added - skip those which were previously already added
        /// </summary>
        /// <returns></returns>
        List<IPageFeature> ManualFeaturesGetNew();

    }
}
