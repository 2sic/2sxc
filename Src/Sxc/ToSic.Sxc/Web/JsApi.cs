using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class JsApi
    {
        private const string MetaTagJsApi = "<meta name=\"_jsApi-temp\" content={0}/>";
        public static string UpdateMetaTagJsApi(string html, string content) => html?.Replace(string.Format(MetaTagJsApi, "\"\""), string.Format(MetaTagJsApi, $"'{content}'"));
    }
}
