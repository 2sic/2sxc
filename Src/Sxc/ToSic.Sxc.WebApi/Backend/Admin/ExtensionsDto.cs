using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Sys.FileSystemState;

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

    [JsonPropertyName("edition")]
    public required string Edition { get; init; } = "";

    [JsonPropertyName("configuration")]
    public required ExtensionManifest Configuration { get; init; }

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = "";
}
