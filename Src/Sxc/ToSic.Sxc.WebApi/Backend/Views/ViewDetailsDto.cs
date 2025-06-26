using System.Text.Json.Serialization;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Sys.Security;

namespace ToSic.Sxc.Backend.Views;

public class ViewDetailsDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required ViewContentTypeDto ContentType { get; init; }
    public required ViewContentTypeDto PresentationType { get; init; }
    public required ViewContentTypeDto ListContentType { get; init; }
    public required ViewContentTypeDto ListPresentationType { get; init; }
    public required string TemplatePath { get; init; }
    public required bool IsHidden { get; init; }
    public required string ViewNameInUrl { get; init; }
    public required Guid Guid { get; init; }
    public required bool List { get; init; }
    public required bool HasQuery { get; init; }
    public required int Used { get; init; }

    public required bool IsShared { get; init; }

    public required EditInfoDto EditInfo { get; init; }


    public required IEnumerable<EavLightEntityReference>? Metadata { get; init; }

    public required HasPermissionsDto Permissions { get; init; }

    [JsonPropertyName("lightSpeed")]
    public required AppMetadataDto? Lightspeed { get; init; }
}