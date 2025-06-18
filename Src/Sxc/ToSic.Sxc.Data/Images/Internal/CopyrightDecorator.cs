using ToSic.Eav.Data.EntityBased.Sys;

namespace ToSic.Sxc.Images.Internal;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
public class CopyrightDecorator(IEntity entity) : EntityBasedType(entity)
{
    public static string TypeNameId = "077835ec-889e-433f-8acf-a4715acb3503";
    public static string NiceTypeName = "CopyrightDecorator";

    public string? CopyrightInfoType => GetThis<string>(null);

    public string? CopyrightMessage => GetThis<string>(null);

    // TODO: THIS will probably not work yet
    public IEnumerable<IEntity>? Copyrights => GetThis<object>(null) as IEnumerable<IEntity>;
}