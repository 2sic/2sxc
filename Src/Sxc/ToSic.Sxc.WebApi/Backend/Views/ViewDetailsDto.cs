using System.Text.Json.Serialization;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.Backend.Views;

public class ViewDetailsDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ViewContentTypeDto ContentType { get; init; }
    public ViewContentTypeDto PresentationType { get; init; }
    public ViewContentTypeDto ListContentType { get; init; }
    public ViewContentTypeDto ListPresentationType { get; init; }
    public string TemplatePath { get; init; }
    public bool IsHidden { get; init; }
    public string ViewNameInUrl { get; init; }
    public Guid Guid { get; init; }
    public bool List { get; init; }
    public bool HasQuery { get; init; }
    public int Used { get; init; }

    public bool IsShared { get; init; }

    public EditInfoDto EditInfo { get; init; }


    public IEnumerable<EavLightEntityReference> Metadata { get; init; }

    public HasPermissionsDto Permissions { get; init; }

    [JsonPropertyName("lightSpeed")]
    public AppMetadataDto Lightspeed { get; init; }
}