using ToSic.Razor.Blade;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Web.Internal.PageService;

public interface IPageServiceShared: IChangeQueue
{
    List<ClientAsset> GetAssetsAndFlush();
    void AddAssets(RenderEngineResult result);
    IPageFeatures PageFeatures { get; }
    CspOfModule Csp { get; }
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
    IEnumerable<string> Activate(params string[] keys);
    //IList<PagePropertyChange> GetPropertyChangesAndFlush(ILog log);
    void Add(IHtmlTag tag, string identifier = null);
}