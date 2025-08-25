using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ClientAssets;
using ToSic.Sxc.Web.Sys.ContentSecurityPolicy;

namespace ToSic.Sxc.Render.Sys;

// TODO: Probably remove this interface, as using the record directly is probably better.

/// <summary>
/// Contains everything which results from a render of a Block
/// Incl. all the features that are activated, page changes etc.
/// It's kind of like a bundle of things the CMS must then do to deliver to the page
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRenderResult
{
    /// <summary>
    /// The resulting HTML to add to the page
    /// </summary>
    string? Html { get; }

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
    IList<IPageFeature>? Features { get; }

    /// <summary>
    /// Assets (js, css) which must be added to the page
    /// </summary>
    IList<ClientAsset>? Assets { get; }

    /// <summary>
    /// Changes to the page properties - like Title, Description, Keywords etc.
    /// </summary>
    IList<PagePropertyChange>? PageChanges { get; }

    /// <summary>
    /// Changes to the Page Header like Meta-Tags etc.
    /// </summary>
    IList<HeadChange>? HeadChanges { get; }

    /// <summary>
    /// Features which are defined in the SystemSettings and wer requested by the code and should be enabled.
    /// </summary>
    IList<PageFeatureFromSettings>? FeaturesFromResources { get; }

    /// <summary>
    /// List of HttpHeaders to add to the response in format "key:value"
    /// </summary>
    IList<HttpHeader>? HttpHeaders { get; }

    /// <summary>
    /// Optional HTTP-Status Code which the code returned.
    /// Typically used on details-pages, which could return a 404 or similar.
    /// If it's applied to the Response, it should probably also include the <see cref="HttpStatusMessage"/>
    /// </summary>
    int? HttpStatusCode { get; }

    /// <summary>
    /// Optional status message which could give the <see cref="HttpStatusCode"/> some context.
    /// </summary>
    string? HttpStatusMessage { get; }

    [PrivateApi]
    List<IDependentApp>? DependentApps { get; }

    [PrivateApi("not in use yet")]
    int ModuleId { get; }


    bool CspEnabled { get; }
    bool CspEnforced { get; }
    List<CspParameters>? CspParameters { get; }

    /// <summary>
    /// Errors such as not-activated features
    /// </summary>
    List<string>? Errors { get; }

    /// <summary>
    /// Info for LightSpeedStats (to group by AppId)
    /// </summary>
    int AppId { get; }

    /// <summary>
    /// Optional caching settings. New 19.03.03
    /// </summary>
    public OutputCacheSettings? OutputCacheSettings { get; init; }

    /// <summary>
    /// Determine if this is just a partial render result, meaning it should be treated differently by the cache.
    /// </summary>
    public bool IsPartial { get; }
}