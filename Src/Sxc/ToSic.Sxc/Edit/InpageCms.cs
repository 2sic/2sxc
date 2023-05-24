using ToSic.Sxc.Web.JsContext;

namespace ToSic.Sxc.Edit
{
    public class InpageCms
    {
        public const string MetaName = "_jsApi";
        public const string ExtensionPlaceholder = "e.x.t";
        public static string JsApiJson(JsApi jsApi) =>
            "{"
            + $"\"{nameof(JsApi.platform)}\": \"{jsApi.platform}\","
            + $"\"{nameof(JsApi.page)}\": {jsApi.page},"
            + $"\"{nameof(JsApi.root)}\": \"{jsApi.root}\","
            + $"\"{nameof(JsApi.api)}\": \"{jsApi.api}\","
            + $"\"{nameof(JsApi.appApi)}\": \"{jsApi.appApi}\", "
            + $"\"{nameof(JsApi.uiRoot)}\": \"{jsApi.uiRoot}\", "
            + $"\"{nameof(JsApi.rvtHeader)}\": \"{jsApi.rvtHeader}\", "
            + $"\"{nameof(JsApi.rvt)}\": \"{jsApi.rvt}\","
            + $"\"{nameof(JsApi.dialogQuery)}\": \"{jsApi.dialogQuery}\""
            + "}";
    }
}
