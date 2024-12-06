using System.Text.Json;
using System.Text.Json.Serialization;

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

    [JsonPropertyName("platform")]
    public string Platform { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("root")]
    public string Root { get; set; }

    [JsonPropertyName("api")]
    public string Api { get; set; }

    [JsonPropertyName("appApi")]
    public string AppApi { get; set; }

    [JsonPropertyName("uiRoot")]
    public string UiRoot { get; set; }

    [JsonPropertyName("rvtHeader")]
    public string RvtHeader { get; set; }

    [JsonPropertyName("rvt")]
    public string Rvt { get; set; }

    [JsonPropertyName("dialogQuery")]
    public string DialogQuery { get; set; }

    [JsonPropertyName("publicKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PublicKey { get; set; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    [JsonIgnore]
    public string Source => "module JsApi";

    public static string JsApiJson(JsApi jsApi) => JsonSerializer.Serialize(jsApi);

}