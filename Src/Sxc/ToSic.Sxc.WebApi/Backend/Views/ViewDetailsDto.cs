using System.Text.Json.Serialization;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Sys.Security;

namespace ToSic.Sxc.Backend.Views;

[ContentType(Name = "View", Guid = "708afea8-33d6-48c0-a629-31a052663bc1", Scope = "System")]
public class ViewDetailsDto : IRawEntityAutoConvert
{
    public required int Id { get; init; }
    [ContentTypeTitle]
    public required string Name { get; init; }
    [ContentTypeField(Type = ValueTypes.Object)]
    public required ViewContentTypeDto ContentType { get; init; }
    [ContentTypeField(Type = ValueTypes.Object)]
    public required ViewContentTypeDto PresentationType { get; init; }
    [ContentTypeField(Type = ValueTypes.Object)]
    public required ViewContentTypeDto ListContentType { get; init; }
    [ContentTypeField(Type = ValueTypes.Object)]
    public required ViewContentTypeDto ListPresentationType { get; init; }
    public required string TemplatePath { get; init; }
    public required bool IsHidden { get; init; }
    public required string ViewNameInUrl { get; init; }
    public required Guid Guid { get; init; }
    public required bool List { get; init; }
    public required bool HasQuery { get; init; }
    public required int Used { get; init; }

    public required bool IsShared { get; init; }

    [ContentTypeField(Type = ValueTypes.Object)]
    public required EditInfoDto EditInfo { get; init; }


    [ContentTypeField(Type = ValueTypes.Object)]
    public required IEnumerable<EavLightEntityReference>? Metadata { get; init; }

    [ContentTypeField(Type = ValueTypes.Object)]
    public required HasPermissionsDto Permissions { get; init; }

    [JsonPropertyName("lightSpeed")]
    [ContentTypeField(Type = ValueTypes.Object)]
    public required AppMetadataDto? Lightspeed { get; init; }
}
