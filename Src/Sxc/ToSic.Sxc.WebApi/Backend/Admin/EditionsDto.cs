using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Code.Generate;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Backend.Admin;

/// <summary>
/// Used to serialize 'editions' to json for UI
/// </summary>
public class EditionsDto: RichResult
{
    public bool IsConfigured { get; init; }
    public ICollection<EditionDto> Editions { get; init; } = [];

    public ICollection<GeneratorDto> Generators { get; init; } = [];
}

public class EditionDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsDefault { get; set; }
}

public class GeneratorDto(IFileGenerator generator)
{
    public string Name => generator.Name;
    public string Version => generator.Version;
    public string Description => generator.Description;
    public string DescriptionHtml => generator.DescriptionHtml;
    public string OutputLanguage => generator.OutputLanguage;
    public string OutputType => generator.OutputType;
}

public static class EditionsJsonExtension
{
    public static EditionsDto ToEditionsDto(this AppJsonConfiguration appJson, ICollection<GeneratorDto> generators)
        => new()
        {
            Ok = true,
            IsConfigured = true,
            Editions = appJson.Editions
                .Select(e => new EditionDto
                {
                    Name = e.Key,
                    Description = e.Value.Description,
                    IsDefault = e.Value.IsDefault
                })
                .ToListOpt(),
            Generators = generators
        };
}