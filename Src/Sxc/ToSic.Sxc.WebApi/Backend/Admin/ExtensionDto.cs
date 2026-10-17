using System.Text.Json.Serialization;
using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Apps.Sys.FileSystemState;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
[ContentType(Name = "AppExtension", Guid = "94687214-88ea-48f7-9153-186c5c885227", Scope = "System")]
public class ExtensionDto : IRawEntityAutoConvert
{
    [ContentTypeTitle]
    [JsonPropertyName("folder")]
    public required string Folder { get; init; }

    [JsonPropertyName("edition")]
    public required string Edition { get; init; } = "";

    [JsonPropertyName("configuration")]
    [ContentTypeField(Type = ValueTypes.Object)]
    public required ExtensionManifest Configuration { get; init; }

    [JsonPropertyName("icon")]
    public string Icon { get; init; } = "";
}
