using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class HtmlDialog
    {
        public const string PageIdInUrl = "pageId";

        public const string CacheBreakPlaceholder = "@sxcver";
        public const string BasePlaceholder = "@base";
        public const string JsApiPlaceholder = "@jsapi";

        public static string UpdatePlaceholders(string html, string content, int pageId)
        {
            if (!html.HasValue()) return "";

            // Add context variables
            var result = html
                // First correct quotes, as the index-raw will always have double quotes (angular-compiler replaces single quotes)
                .Replace($"\"{JsApiPlaceholder}\"", $"'{JsApiPlaceholder}'")
                .Replace(JsApiPlaceholder, content);

            // Add version cachebreak
            result = result.Replace(CacheBreakPlaceholder, EavSystemInfo.VersionWithStartUpBuild);

            // Replace base url
            result = result.Replace(BasePlaceholder, $"./?{PageIdInUrl}={pageId}");

            return result;
        }
    }
}
