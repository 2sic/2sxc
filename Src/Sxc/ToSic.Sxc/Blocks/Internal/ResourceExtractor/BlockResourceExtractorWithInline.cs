using ToSic.Lib.Helpers;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// ATM only used in Oqtane, where external and internal scripts must be extracted
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockResourceExtractorWithInline(PageServiceShared pageServiceShared)
    : BlockResourceExtractor(pageServiceShared)
{
    protected override ClientAssetsExtractSettings Settings => _settings.Get(() => new(extractAll: true));
    private readonly GetOnce<ClientAssetsExtractSettings> _settings = new();

    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
    {
        var include2SxcJs = false;

        // Handle Client Dependency injection
        html = ExtractExternalScripts(html, ref include2SxcJs, settings);

        // Handle inline JS
        html = ExtractInlineScripts(html);

        // Handle Styles
        html = ExtractStyles(html, settings);

        Assets.ForEach(a => a.PosInPage = PositionNameUnchanged(a.PosInPage));

        return (html, include2SxcJs);
    }



    private string PositionNameUnchanged(string position)
    {
        position = position.ToLowerInvariant();

        switch (position)
        {
            case "body":
            case "head":
                return position;
            default:
                return "body";
        }
    }

}