using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInstallPreflightResultDto
{
    [JsonPropertyName("extensions")]
    public List<ExtensionInstallPreflightExtensionDto> Extensions { get; init; } = [];
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInstallPreflightExtensionDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("version")]
    public string? Version { get; init; }

    [JsonPropertyName("editionsSupported")]
    public bool EditionsSupported { get; init; }

    [JsonPropertyName("fileCount")]
    public int FileCount { get; init; }

    [JsonPropertyName("features")]
    public ExtensionInstallPreflightFeaturesDto Features { get; init; } = new();

    [JsonPropertyName("editions")]
    public List<ExtensionInstallPreflightEditionDto> Editions { get; init; } = [];
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInstallPreflightEditionDto
{
    [JsonPropertyName("edition")]
    public string Edition { get; init; } = string.Empty;

    [JsonPropertyName("isInstalled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsInstalled { get; init; } = null;

    [JsonPropertyName("currentVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CurrentVersion { get; init; } = null;

    [JsonPropertyName("hasFileChanges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? HasFileChanges { get; init; } = null;

    [JsonPropertyName("hasData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? HasData { get; init; } = null;

    [JsonPropertyName("breakingChanges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? BreakingChanges { get; init; } = null;
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInstallPreflightFeaturesDto
{
    [JsonPropertyName("fieldsInside")]
    public bool FieldsInside { get; init; }

    [JsonPropertyName("razorInside")]
    public bool RazorInside { get; init; }

    [JsonPropertyName("appCodeInside")]
    public bool AppCodeInside { get; init; }

    [JsonPropertyName("webApiInside")]
    public bool WebApiInside { get; init; }

    [JsonPropertyName("contentTypesInside")]
    public bool ContentTypesInside { get; init; }

    [JsonPropertyName("dataBundlesInside")]
    public bool DataBundlesInside { get; init; }

    [JsonPropertyName("queriesInside")]
    public bool QueriesInside { get; init; }

    [JsonPropertyName("viewsInside")]
    public bool ViewsInside { get; init; }

    [JsonPropertyName("dataInside")]
    public bool DataInside { get; init; }

    [JsonPropertyName("inputTypeInside")]
    public bool InputTypeInside { get; init; }
}
