using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Admin;

public class ScopeDetailsDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("label")]
    public required string Label { get; set; }
    [JsonPropertyName("typesTotal")]
    public required int TypesTotal { get; set; }
    [JsonPropertyName("typesOfApp")]
    public required int TypesOfApp { get; set; }
    [JsonPropertyName("typesInherited")]
    public required int TypesInherited { get; set; }
}

public class ScopesDto
{
    [JsonPropertyName("old")]
    public required IDictionary<string, string> Old { get; set; }
    [JsonPropertyName("scopes")]
    public required List<ScopeDetailsDto> Scopes { get; set; }
}