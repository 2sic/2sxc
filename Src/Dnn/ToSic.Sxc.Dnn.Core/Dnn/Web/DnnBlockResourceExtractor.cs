using DotNetNuke.Web.Client;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnBlockResourceExtractor: BlockResourceExtractor
    {
        public DnnBlockResourceExtractor(PageServiceShared pageServiceShared): base(pageServiceShared) { }

        protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate)
        {
            var wrapLog = Log.Call<(string, bool)>();

            // Set priority for later processing?
            JsDefaultPriority = (int)FileOrder.Js.DefaultPriority;
            CssDefaultPriority = (int)FileOrder.Css.DefaultPriority;

            //var page = HttpContext.Current.CurrentHandler as Page;
            var include2SxcJs = false;
            
            // Handle Client Dependency injection
            renderedTemplate = ExtractExternalScripts(renderedTemplate, ref include2SxcJs);

            // Handle Scripts
            renderedTemplate = ExtractStyles(renderedTemplate);

            return wrapLog("ok", (renderedTemplate, include2SxcJs));
        }
    }
}