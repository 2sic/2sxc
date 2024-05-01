using DotNetNuke.Web.Client;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnBlockResourceExtractor(PageServiceShared pageServiceShared)
    : BlockResourceExtractor(pageServiceShared)
{
    protected override ClientAssetsExtractSettings Settings => _settings.Get(() => new(
        extractAll: false,
        cssPriority: (int)FileOrder.Css.DefaultPriority,
        jsPriority: (int)FileOrder.Js.DefaultPriority));
    private readonly GetOnce<ClientAssetsExtractSettings> _settings = new();


    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
    {
        var l = Log.Fn<(string, bool)>();
        var include2SxcJs = false;
            
        // Handle Client Dependency injection
        html = ExtractExternalScripts(html, ref include2SxcJs, settings);

        // Handle Scripts
        html = ExtractStyles(html, settings);

        return l.ReturnAsOk((html, include2SxcJs));
    }
}