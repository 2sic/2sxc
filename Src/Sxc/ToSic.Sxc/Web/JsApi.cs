using ToSic.Eav;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class JsApi
    {
        private const string MetaTagJsApiTemp = "<meta name=\"_jsApi-temp\" content={0}";
        private const string MetaTagJsApi = "<meta name=\"_jsApi\" content={0}";
        private const string CacheBreakPlaceholder = "@sxcver";

        public static string UpdateMetaTagJsApi(string html, string content)
        {
            // Add context variables
            var result = html?
                .Replace(string.Format(MetaTagJsApi, "\"\""), string.Format(MetaTagJsApi, $"'{content}'"))
                .Replace(string.Format(MetaTagJsApiTemp, "\"\""), string.Format(MetaTagJsApiTemp, $"'{content}'"));

            // Add version cachebreak
            result = result?.Replace(CacheBreakPlaceholder, EavSystemInfo.VersionWithStartUpBuild);
            return result;
        }
    }
}
