// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006
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
    public string platform { get; set; }

    [JsonPropertyName("page")]
    public int page { get; set; }

    [JsonPropertyName("root")]
    public string root { get; set; }

    [JsonPropertyName("api")]
    public string api { get; set; }

    [JsonPropertyName("appApi")]
    public string appApi { get; set; }

    [JsonPropertyName("uiRoot")]
    public string uiRoot { get; set; }

    [JsonPropertyName("rvtHeader")]
    public string rvtHeader { get; set; }

    [JsonPropertyName("rvt")]
    public string rvt { get; set; }

    [JsonPropertyName("dialogQuery")]
    public string dialogQuery { get; set; }

    // TODO: should probably get a nicer name, like "publicKey" or "publicKeyForEncryption"
    // ...but change would require JS changes as well

    [JsonPropertyName("secureEndpointPublicKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string secureEndpointPublicKey { get; set; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    [JsonIgnore]
    public string source => "module JsApi";

    public static string JsApiJson(JsApi jsApi) => JsonSerializer.Serialize(jsApi);

}