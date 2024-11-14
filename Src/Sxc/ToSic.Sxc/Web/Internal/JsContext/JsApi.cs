// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
using System.Text.Json;

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
    public string secureEndpointPublicKey { get; set; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    public string source => "module JsApi";

    public static string JsApiJson(JsApi jsApi) =>
        "{"
        + JsonProperty(nameof(JsApi.platform), jsApi.platform) + ","
        + $"\"{nameof(JsApi.page)}\": {jsApi.page},"
        + JsonProperty(nameof(JsApi.root), jsApi.root) + ","
        + JsonProperty(nameof(JsApi.api), jsApi.api) + ","
        + JsonProperty(nameof(JsApi.appApi), jsApi.appApi) + ","
        + JsonProperty(nameof(JsApi.uiRoot), jsApi.uiRoot) + ","
        + JsonProperty(nameof(JsApi.rvtHeader), jsApi.rvtHeader) + ","
        + JsonProperty(nameof(JsApi.rvt), jsApi.rvt) + ","
        + JsonProperty(nameof(JsApi.dialogQuery), jsApi.dialogQuery) + ","
        + JsonPropertyOrEmpty(nameof(jsApi.secureEndpointPublicKey), jsApi.secureEndpointPublicKey)
        + "}";

    private static string JsonPropertyOrEmpty(string key, string value)
        => string.IsNullOrEmpty(value) ? string.Empty : JsonProperty(key, value);

    /// <summary>
    /// build string for json property
    /// "key: 'value'"
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string JsonProperty(string key, string value)
        => $"\"{key}\": \"{JsonValue(value)}\"";

    /// <summary>
    /// build string for json encoded value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string JsonValue(string value)
        => string.IsNullOrEmpty(value) ? string.Empty : JsonEncodedText.Encode(value).ToString();
}