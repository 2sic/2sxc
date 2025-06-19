using System.Text.Json.Serialization;

namespace ToSic.Sxc.Web.Internal.JsContext;

/// <summary>
/// This is a special json-structure which will be added to the page head.
/// It's necessary for API calls to just-work, since the JS needs to know the API-URLs and more.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class JsApi
{
    public const string MetaName = "_jsApi";
    public const string ExtensionPlaceholder = "e.x.t";

    [JsonPropertyName("platform")]
    public required string Platform { get; init; }

    [JsonPropertyName("page")]
    public required int Page { get; init; }

    [JsonPropertyName("root")]
    public required string Root { get; init; }

    [JsonPropertyName("api")]
    public required string Api { get; init; }

    [JsonPropertyName("appApi")]
    public required string AppApi { get; init; }

    [JsonPropertyName("uiRoot")]
    public required string UiRoot { get; init; }

    [JsonPropertyName("rvtHeader")]
    public required string RvtHeader { get; init; }

    [JsonPropertyName("rvt")]
    public required string Rvt { get; init; }

    [JsonPropertyName("dialogQuery")]
    public required string? DialogQuery { get; init; }

    [JsonPropertyName("publicKey")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? PublicKey { get; init; }

    /// <summary>
    /// Debug information while we're developing the on-module info
    /// </summary>
    [JsonIgnore]
    public string Source => "module JsApi";
}