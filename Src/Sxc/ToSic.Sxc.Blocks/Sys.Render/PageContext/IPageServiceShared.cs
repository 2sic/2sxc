using ToSic.Razor.Blade;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Sys.Render.PageFeatures;
using ToSic.Sxc.Web.Sys.ClientAssets;

namespace ToSic.Sxc.Sys.Render.PageContext;

public interface IPageServiceShared: IChangeQueue
{
    List<ClientAsset> GetAssetsAndFlush();
    void AddAssets(RenderEngineResult result);
    IPageFeatures PageFeatures { get; }

    string CspEphemeralMarker { get; }

    /// <summary>
    /// How the changes given to this object should be processed.
    /// </summary>
    PageChangeModes ChangeMode { get; set; }

    List<HeadChange> Headers { get; }

    List<HttpHeader> HttpHeaders { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="identifier"></param>
    /// <returns>The thing added to the head - or null if nothing added because tag was null</returns>
    HeadChange? Add(IHtmlTag tag, string? identifier = null);
}