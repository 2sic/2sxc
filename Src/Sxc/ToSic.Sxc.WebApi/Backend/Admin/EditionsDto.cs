using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Admin;

/// <summary>
/// Used to serialize 'editions' to json for UI
/// </summary>
public class EditionsDto: RichResult
{
    public bool IsConfigured { get; set; }
    public List<EditionDto> Editions { get; set; } = [];
}

public class EditionDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsDefault { get; set; }
}