namespace ToSic.Sxc.Edit
{
    public class InpageCms
    {
        public const string MetaName = "_jsApi";
        public const string ExtensionPlaceholder = "e.x.t";

        public const string CoreJs = "js/2sxc.api.min.js";

        public const string EditJs = "dist/inpage/inpage.min.js";
        public const string EditCss = "dist/inpage/inpage.min.css";

        public static string JsApiJson(string platform, int pageId, string siteRoot, string apiRoot, string appApiRoot, string rvt, string uiRoot)
        {
            var json = "{"
                       + $"\"platform\": \"{platform.ToLowerInvariant()}\","
                       + $"\"page\": {pageId},"
                       + $"\"root\": \"{siteRoot}\","
                       + $"\"api\": \"{apiRoot}\","
                       + $"\"appApi\": \"{appApiRoot}\", "
                       + $"\"rvt\": \"{rvt}\", "
                       + $"\"uiRoot\": \"{uiRoot}\""
                       + "}";
            return json;
        }
    }
}
