using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectResultDto
{
    [JsonPropertyName("foundLock")]
    public bool FoundLock { get; init; }

    [JsonPropertyName("files")]
    public List<ExtensionFileStatusDto>? Files { get; init; }

    [JsonPropertyName("summary")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExtensionInspectSummaryDto? Summary { get; init; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExtensionInspectDataDto? Data { get; init; }
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionFileStatusDto
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; } // unchanged | changed | added | missing
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectSummaryDto
{
    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("changed")]
    public int Changed { get; init; }

    [JsonPropertyName("added")]
    public int Added { get; init; }

    [JsonPropertyName("missing")]
    public int Missing { get; init; }
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectDataDto
{
    [JsonPropertyName("contentTypes")]
    public List<ExtensionInspectContentTypeDto> ContentTypes { get; init; } = [];
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectContentTypeDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("guid")]
    public required string Guid { get; init; }

    [JsonPropertyName("localEntities")]
    public int LocalEntities { get; init; }
}
