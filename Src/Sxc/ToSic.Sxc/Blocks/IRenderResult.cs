using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks
{
    /// <summary>
    /// Contains everything which results from a render of a Block
    /// Incl. all the features that are activated, page changes etc.
    /// It's kind of like a bundle of things the CMS must then do to deliver to the page
    /// </summary>
    public interface IRenderResult
    {
        /// <summary>
        /// The resulting HTML to add to the page
        /// </summary>
        string Html { get; }

        /// <summary>
        /// Built-in page features (like jQuery, 2sxc.JsCode, ...) which were requested by the code and should be enabled
        /// </summary>
        IList<IPageFeature> Features { get; }

        /// <summary>
        /// Assets (js, css) which must be added to the page
        /// </summary>
        IList<IClientAsset> Assets { get; }

        /// <summary>
        /// Changes to the page properties - like Title, Description, Keywords etc.
        /// </summary>
        IList<PagePropertyChange> PageChanges { get; }

        /// <summary>
        /// Changes to the Page Header like Meta-Tags etc.
        /// </summary>
        IList<HeadChange> HeadChanges { get; }

        /// <summary>
        /// Features which are defined in the SystemSettings and wer requested by the code and should be enabled.
        /// </summary>
        IList<IPageFeature> FeaturesFromSettings { get; }

        /// <summary>
        /// Optional HTTP-Status Code which the code returned.
        /// Typically used on details-pages, which could return a 404 or similar.
        /// If it's applied to the Response, it should probably also include the <see cref="HttpStatusMessage"/>
        /// </summary>
        int? HttpStatusCode { get; }

        /// <summary>
        /// Optional status message which could give the <see cref="HttpStatusCode"/> some context.
        /// </summary>
        string HttpStatusMessage { get; }

        [PrivateApi]
        IList<int> DependentApps { get; }

        [PrivateApi("not in use yet")]
        int ModuleId { get; }
    }
}