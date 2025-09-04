using DotNetNuke.Web.Client;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Render.Sys.ResourceExtractor;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnBlockResourceExtractor(IPageServiceShared pageServiceShared)
    : BlockResourceExtractor(pageServiceShared)
{
    private const bool DebugDetails = true;

    protected override ClientAssetsExtractSettings DefaultSettings => _settings.Get(() => new(
        extractAll: false,
        cssPriority: (int)FileOrder.Css.DefaultPriority,
        jsPriority: (int)FileOrder.Js.DefaultPriority));
    private readonly GetOnce<ClientAssetsExtractSettings> _settings = new();


    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
    {
        var l = Log.Fn<(string, bool)>();
        var include2SxcJs = false;
            
        // Handle Client Dependency injection
        html = ExtractExternalScripts(html, ref include2SxcJs, settings, logDetails: DebugDetails); // 2025-09-04 2dm having some difficulties, want to log details

        // Handle Scripts
        html = ExtractStyles(html, settings);

        return l.ReturnAsOk((html, include2SxcJs));
    }
}