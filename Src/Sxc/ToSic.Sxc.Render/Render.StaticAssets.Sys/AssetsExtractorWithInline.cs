using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Render.StaticAssets.Sys;

/// <summary>
/// ATM only used in Oqtane, where external and internal scripts must be extracted
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AssetsExtractorWithInline(IPageServiceShared pageServiceShared)
    : AssetsExtractor(pageServiceShared)
{
    protected override ClientAssetsExtractSettings DefaultSettings => field ??= new(extractAll: true);

    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
    {
        var include2SxcJs = false;

        // Handle Client Dependency injection
        html = ExtractExternalScripts(html, ref include2SxcJs, settings);

        // Handle inline JS
        html = ExtractInlineScripts(html);

        // Handle Styles
        html = ExtractStyles(html, settings);

        // 2025-03-17 optimized to functional - remove comment in a few weeks
        //Assets.ForEach(a => a.PosInPage = PositionNameUnchanged(a.PosInPage));
        Assets = Assets
            .Select(a => a with { PosInPage = PositionNameUnchanged(a.PosInPage) })
            .ToList();

        return (html, include2SxcJs);
    }



    private string PositionNameUnchanged(string position)
    {
        position = position.ToLowerInvariant();

        return position switch
        {
            "body" or "head" => position,
            _ => "body"
        };
    }

}