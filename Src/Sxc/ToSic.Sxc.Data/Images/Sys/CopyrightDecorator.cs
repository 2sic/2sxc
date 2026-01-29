using ToSic.Eav.Model;

namespace ToSic.Sxc.Images.Sys;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
[ModelSource(ContentType = ContentTypeNameId)]
public record CopyrightDecorator : ModelOfEntityCore
{
    public const string ContentTypeNameId = "077835ec-889e-433f-8acf-a4715acb3503";
    public const string ContentTypeName = "CopyrightDecorator";

    public string? CopyrightInfoType => GetThis<string>(null);

    public string? CopyrightMessage => GetThis<string>(null);

    // TODO: THIS will probably not work yet
    public IEnumerable<IEntity>? Copyrights => GetThis<object>(null) as IEnumerable<IEntity>;
}