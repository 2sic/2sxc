using System.Text.Json.Serialization;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionsResultDto
{
    [JsonPropertyName("extensions")]
    public ICollection<ExtensionDto> Extensions { get; init; } = [];
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionDto
{
    [JsonPropertyName("folder")]
    public required string Folder { get; init; }

    [JsonPropertyName("configuration")]
    public required object Configuration { get; init; }
}
