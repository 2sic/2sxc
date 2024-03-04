using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Admin;

public class ScopeDetailsDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("label")]
    public string Label { get; set; }
    [JsonPropertyName("typesTotal")]
    public int TypesTotal { get; set; }
    [JsonPropertyName("typesOfApp")]
    public int TypesOfApp { get; set; }
    [JsonPropertyName("typesInherited")]
    public int TypesInherited { get; set; }
}

public class ScopesDto
{
    [JsonPropertyName("old")]
    public IDictionary<string, string> Old { get; set; }
    [JsonPropertyName("scopes")]
    public List<ScopeDetailsDto> Scopes { get; set; }
}