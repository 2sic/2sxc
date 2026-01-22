using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Images.Sys;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
public record CopyrightDecorator : RecordOfEntityBase
{
    public CopyrightDecorator(IEntity entity) : base(entity) { }

    public static string TypeNameId = "077835ec-889e-433f-8acf-a4715acb3503";
    public static string NiceTypeName = "CopyrightDecorator";

    public string? CopyrightInfoType => GetThis<string>(null);

    public string? CopyrightMessage => GetThis<string>(null);

    // TODO: THIS will probably not work yet
    public IEnumerable<IEntity>? Copyrights => GetThis<object>(null) as IEnumerable<IEntity>;
}