using System.Text.Json.Serialization;

namespace ToSic.Sxc.Services.Internal;

internal class TurnOnSpecs
{
    [JsonPropertyName("run")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Run { get; init; }

    [JsonPropertyName("require")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Require { get; init; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object Data { get; init; }

    [JsonPropertyName("args")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<object> Args { get; init; }

    [JsonPropertyName("addContext")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AddContext { get; init; }
}