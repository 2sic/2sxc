// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
namespace ToSic.Sxc.Web.Internal.JsContext;

/// <summary>
/// This is a special json-structure which will be added to the page head.
/// It's necessary for API calls to just-work, since the JS needs to know the API-URLs and more.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class JsApi
{
    public const string MetaName = "_jsApi";
    public const string ExtensionPlaceholder = "e.x.t";


    public string platform { get; set; }
    public int page { get; set; }
    public string root { get; set; }
    public string api { get; set; }
    public string appApi { get; set; }
    public string uiRoot { get; set; }
    public string rvtHeader { get; set; }
    public string rvt { get; set; }
    public string dialogQuery { get; set; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    public string source => "module JsApi";

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