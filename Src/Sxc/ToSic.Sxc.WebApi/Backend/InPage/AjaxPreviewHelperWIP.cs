using ToSic.Eav;
using ToSic.Eav.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Backend.InPage;

/// <summary>
/// This helps the ajax preview to ensure js/css are also loaded in the preview.
/// - v0.1 for 2sxc 12.05 will just rebuild the necessary tags and add them to the HTML
/// - v0.2 should later on include the relevant assets so that the JS in the can add them as best possible
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AjaxPreviewHelperWIP
{
    public string ReconstructHtml(IRenderResult renderResult, string root)
    {
        // 0. Skip basics like jQuery, $2sxc, editApi and editUI as they are always available in edit mode

        // 0.1 Get Version etc.
        root = root.SuffixSlash(); // important because DNN historically has a slash in the constant, Oqtane does not
        var ver = EavSystemInfo.VersionWithStartUpBuild; // Settings.Version.ToString();
        var addOn = "";

        // 1. Check if the features includes turnOn
        if (renderResult.Features.Contains(SxcPageFeatures.TurnOn)) 
            addOn += Js(ver, root + SxcPageFeatures.TurnOn.UrlInDist);

        // 2. Add JS & CSS which was stripped before
        renderResult.Assets.ToList().ForEach(a => addOn += "\n" + (a.IsJs ? Js(ver, a.Url) : Css(a.Url)));

        renderResult.FeaturesFromSettings.ToList().ForEach(f => addOn += "\n" + f.Html);

        var html = renderResult.Html;
        if (!string.IsNullOrEmpty(html) && !string.IsNullOrEmpty(addOn))
        {
            // Must insert before the last closing div
            var lastDiv = html.LastIndexOf("</div>", StringComparison.InvariantCultureIgnoreCase);
            var result = html.Substring(0, lastDiv)
                         + "\n<!-- resources added for ajax -->"
                         + addOn
                         + "\n"
                         + "</div>";
            html = result;
        }

        return html; 
    }

    private static string Js(string version, string path)
    {
        var url = $"{path}{(path.IndexOf('?') > 0 ? '&' : '?')}v={version}";
        return Tag.Script().Src(url).Type("text/javascript").ToString();
    }

    private static string Css(string path)
        => Tag.Link().Href(path).Type("text/css").Rel("stylesheet").ToString();

}