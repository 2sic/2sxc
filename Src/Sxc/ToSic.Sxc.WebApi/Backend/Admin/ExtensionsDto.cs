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

    /// <summary>
    /// Edition-specific information if the extension supports editions.
    /// Null if editions are not supported or not found.
    /// </summary>
    [JsonPropertyName("editions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, ExtensionEditionDto>? Editions { get; init; }
}

/// <summary>
/// Information about a specific edition of an extension.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionEditionDto
{
    /// <summary>
    /// The folder name for this edition (e.g., "staging", "live")
    /// </summary>
    [JsonPropertyName("folder")]
    public required string Folder { get; init; }

    /// <summary>
    /// The configuration object for this edition
    /// </summary>
    [JsonPropertyName("configuration")]
    public required object Configuration { get; init; }
}
