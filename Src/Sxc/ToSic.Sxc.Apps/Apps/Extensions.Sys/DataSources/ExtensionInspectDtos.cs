using System.Text.Json.Serialization;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;

namespace ToSic.Sxc.Apps.Extensions.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectResultGroup
{
    [JsonPropertyName("foundLock")]
    public bool FoundLock { get; init; }

    [JsonPropertyName("files")]
    public List<ExtensionFileStatusRaw>? Files { get; init; }

    [JsonPropertyName("summary")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExtensionInspectSummaryRaw? Summary { get; init; }

    [JsonPropertyName("contentTypes")]
    public List<ExtensionInspectContentTypeRaw> ContentTypes { get; init; } = [];
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionFileStatusRaw : IRawEntityAutoConvert
{
    [ContentTypeTitle]
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; } // unchanged | changed | added | missing
}

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionInspectSummaryRaw : IRawEntityAutoConvert
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
public class ExtensionInspectContentTypeRaw : IRawEntityAutoConvert
{
    [ContentTypeTitle]
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("guid")]
    public required string Guid { get; init; }

    [JsonPropertyName("localEntities")]
    public int LocalEntities { get; init; }
}
