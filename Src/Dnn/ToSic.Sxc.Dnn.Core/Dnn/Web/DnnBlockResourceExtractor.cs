using System;
using DotNetNuke.Web.Client;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnBlockResourceExtractor: BlockResourceExtractor
    {

        public override (string Template, bool Include2sxcJs) Process(string renderedTemplate)
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