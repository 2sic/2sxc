using DotNetNuke.Web.Client;
using ToSic.Sxc.Render.StaticAssets.Sys;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnAssetsExtractor(IPageServiceShared pageServiceShared)
    : AssetsExtractor(pageServiceShared)
{
    // 2025-09-04 2dm having some difficulties, want to log details
    // 2026-08-13 2dm disabled again, clogs up logs and memory
    // if we think this should be used from time to time, we should create an option
    private const bool DebugDetails = true;

    protected override ClientAssetsExtractSettings DefaultSettings => field
        ??= new(
            extractAll: false,
            cssPriority: (int)FileOrder.Css.DefaultPriority,
            jsPriority: (int)FileOrder.Js.DefaultPriority
        );


    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
    {
        var l = Log.Fn<(string, bool)>();
        var include2SxcJs = false;
            
        // Handle Client Dependency injection
        html = ExtractExternalScripts(html, ref include2SxcJs, settings, logDetails: DebugDetails);

        // Handle Scripts
        html = ExtractStyles(html, settings);

        return l.ReturnAsOk((html, include2SxcJs));
    }
}