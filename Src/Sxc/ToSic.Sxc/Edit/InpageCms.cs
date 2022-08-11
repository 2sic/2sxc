namespace ToSic.Sxc.Edit
{
    public class InpageCms
    {
        public const string MetaName = "_jsApi";
        public const string ExtensionPlaceholder = "e.x.t";

        public const string CoreJs = "js/2sxc.api.min.js";

        public const string EditJs = "dist/inpage/inpage.min.js";
        public const string EditCss = "dist/inpage/inpage.min.css";
        
        // WIP
        public const string TurnOnJs = "dist/turnOn/turn-on.js";

        public static string JsApiJson(string platform,
            int pageId, 
            string siteRoot, 
            string apiRoot, 
            string appApiRoot,
            string uiRoot, 
            string rvtHeader, 
            string rvt,
            string dialogQuery = null // these are any platform specific url query params to the dialog; can be null
        )
        {
            var json = "{"
                       + $"\"platform\": \"{platform.ToLowerInvariant()}\","
                       + $"\"page\": {pageId},"
                       + $"\"root\": \"{siteRoot}\","
                       + $"\"api\": \"{apiRoot}\","
                       + $"\"appApi\": \"{appApiRoot}\", "
                       + $"\"uiRoot\": \"{uiRoot}\", "
                       + $"\"rvtHeader\": \"{rvtHeader}\", "
                       + $"\"rvt\": \"{rvt}\","
                       + $"\"dialogQuery\": \"{dialogQuery}\""
                       + "}";
            return json;
        }
    }
}
