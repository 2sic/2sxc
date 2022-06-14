using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class HtmlDialog
    {
        public const string PageIdInUrl = "pageId";

        private const string MetaTagJsApiTemp = "<meta name=\"_jsApi-temp\" content={0}";
        private const string MetaTagJsApi = "<meta name=\"_jsApi\" content={0}";
        private const string CacheBreakPlaceholder = "@sxcver";

        private const string OriginalBase = "<base href=\"./\">";
        private const string NewBase = "<base href=\"{0}\">";

        public static string UpdatePlaceholders(string html, string content, int pageId)
        {
            if (!html.HasValue()) return "";

            // Add context variables
            var result = html
                .Replace(string.Format(MetaTagJsApi, "\"\""), string.Format(MetaTagJsApi, $"'{content}'"))
                .Replace(string.Format(MetaTagJsApiTemp, "\"\""), string.Format(MetaTagJsApiTemp, $"'{content}'"));

            // Add version cachebreak
            result = result.Replace(CacheBreakPlaceholder, EavSystemInfo.VersionWithStartUpBuild);

            result = result.Replace(OriginalBase, string.Format(NewBase, $"./?{PageIdInUrl}={pageId}"));

            return result;
        }
    }
}
