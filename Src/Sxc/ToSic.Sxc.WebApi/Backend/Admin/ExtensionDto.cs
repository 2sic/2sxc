using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Apps.Sys.FileSystemState;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ExtensionDto : IRawEntityAutoConvert
{
    [ContentTypeTitle]
    [JsonPropertyName("folder")]
    public required string Folder { get; init; }

    [JsonPropertyName("edition")]
    public required string Edition { get; init; } = "";

    [JsonPropertyName("configuration")]
    public required ExtensionManifest Configuration { get; init; }

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = "";
}
