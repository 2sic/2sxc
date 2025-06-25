using ToSic.Razor.Blade;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Internal.ClientAssets;

namespace ToSic.Sxc.Sys.Render.PageContext;

public interface IPageServiceShared: IChangeQueue
{
    List<ClientAsset> GetAssetsAndFlush();
    void AddAssets(RenderEngineResult result);
    IPageFeatures PageFeatures { get; }

    // Moving CspOfModule to internal
    //CspOfModule Csp { get; }
    string CspEphemeralMarker { get; }

    /// <summary>
    /// How the changes given to this object should be processed.
    /// </summary>
    PageChangeModes ChangeMode { get; set; }

    IList<HeadChange> Headers { get; }
    //int? HttpStatusCode { get; set; }
    //string HttpStatusMessage { get; set; }
    List<HttpHeader> HttpHeaders { get; }
    //IList<HeadChange> GetHeadChangesAndFlush(ILog log);

    /// <summary>
    /// Register the feature keys to be activated on the page.
    /// </summary>
    /// <param name="keys"></param>
    /// <returns>The keys it just activated (after trim/filter for empty)</returns>
    IEnumerable<string> Activate(params string[] keys);
    //IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log);
    void Add(IHtmlTag tag, string? identifier = null);
}