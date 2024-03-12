namespace ToSic.Sxc.Backend.Admin;

// TODO: @STV - this should be moved to an AppJsonService in Eav.Apps

/// <summary>
/// Used to deserialize 'editions' from app.json
/// </summary>
public class EditionsJson
{
    public bool IsConfigured { get; set; }
    public Dictionary<string, EditionInfo> Editions { get; set; } = [];
}

public class EditionInfo(string description)
{
    public string Description { get; set; } = description;
    public bool IsDefault { get; set; }
}

public static class EditionsJsonExtension
{
    public static EditionsDto ToEditionsDto(this EditionsJson editionsJson, List<GeneratorDto> generators)
        => new()
        {
            Ok = true,
            IsConfigured = true,
            Editions = editionsJson.Editions
                .Select(e => new EditionDto
                {
                    Name = e.Key,
                    Description = e.Value.Description,
                    IsDefault = e.Value.IsDefault
                })
                .ToList(),
            Generators = generators
        };
}