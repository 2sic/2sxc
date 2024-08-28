using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Blocks.Internal.Render;

/// <summary>
/// Contains everything which results from a render of a Block
/// Incl. all the features that are activated, page changes etc.
/// It's kind of like a bundle of things the CMS must then do to deliver to the page
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IRenderResult
{
    /// <summary>
    /// The resulting HTML to add to the page
    /// </summary>
    string Html { get; }

    /// <summary>
    /// WIP - should tell us how big this is in memory - estimate
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Determines if this render-result can be cached.
    /// Should be false in case of errors or not-yet initialized content
    /// </summary>
    bool CanCache { get; }

    /// <summary>
    /// Information that the result contains an error message and should be treated differently, like no caching
    /// </summary>
    bool IsError { get; }

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
    /// List of HttpHeaders to add to the response in format "key:value"
    /// </summary>
    //IList<string> HttpHeaders { get; }
    IList<HttpHeader> HttpHeaders { get; }

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
    List<IDependentApp> DependentApps { get; }

    [PrivateApi("not in use yet")]
    int ModuleId { get; }


    bool CspEnabled { get; }
    bool CspEnforced { get; }
    IList<CspParameters> CspParameters { get; }

    /// <summary>
    /// Errors such as not-activated features
    /// </summary>
    List<string> Errors { get; set; }

    /// <summary>
    /// Info for LightSpeedStats (to group by AppId)
    /// </summary>
    int AppId { get; set; }
}