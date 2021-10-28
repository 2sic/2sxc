using System;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Web.PageFeatures;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    /// <summary>
    /// This helps the ajax preview to ensure js/css are also loaded in the preview.
    /// - v0.1 for 2sxc 12.05 will just rebuild the necessary tags and add them to the HTML
    /// - v0.2 should later on include the relevant assets so that the JS in the can add them as best possible
    /// </summary>
    public class AjaxPreviewHelperWIP
    {
        public string ReconstructHtml(RenderResultWIP renderResult)
        {
            // 0. Skip basics like jQuery, $2sxc, editApi and editUI as they are always available in edit mode

            // 0.1 Get root paths
            var root = DnnConstants.SysFolderRootVirtual.Trim('~');
            var ver = Settings.Version.ToString();
            var addOn = "";

            // 1. Check if the features includes turnOn
            if (renderResult.Features.Contains(BuiltInFeatures.TurnOn)) 
                addOn += Js(ver, root + InpageCms.TurnOnJs);

            // 2. Add JS & CSS which was stripped before
            renderResult.Assets.ForEach(a => addOn += a.IsJs ? Js(ver, a.Url) : Css(a.Url));

            var html = renderResult.Html;
            if (!string.IsNullOrEmpty(html) && !string.IsNullOrEmpty(addOn))
            {
                // Must insert before the last closing div
                var lastDiv = html.LastIndexOf("</div>", StringComparison.InvariantCultureIgnoreCase);
                var result = html.Substring(0, lastDiv)
                    + "\n<!-- resources added for ajax -->\n"
                    + addOn
                    + "\n"
                    + "</div>";
                html = result;
            }

            return html; 
        }

        private static string Js(string version, string path)
        {
            var url = UrlHelpers.QuickAddUrlParameter(path, "v", version);
            return Tag.Script().Src(url).Type("text/javascript").ToString();
        }

        private static string Css(string path)
            => Tag.Link().Href(path).Type("text/css").Rel("stylesheet").ToString();

    }
}