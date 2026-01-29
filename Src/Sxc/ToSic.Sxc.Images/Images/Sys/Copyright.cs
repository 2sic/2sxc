using ToSic.Eav.Model;

namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// TODO: unclear what this is for
/// </summary>
// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
[ModelSource(ContentType = ContentTypeNameId)]
public record Copyright : ModelOfEntityCore
{
    public const string ContentTypeNameId = "ac3df5f0-c637-45e7-a52b-b323d50e52ac";
    public const string ContentTypeName = "🖺Copyright";

    public string? CopyrightType => GetThis<string>(null);

    public string? CopyrightMessage => GetThis<string>(null);

    public int CopyrightYear => GetThis(0);

    public string? CopyrightOwner => GetThis<string>(null);

    public string? CopyrightLink => GetThis<string>(null);

}