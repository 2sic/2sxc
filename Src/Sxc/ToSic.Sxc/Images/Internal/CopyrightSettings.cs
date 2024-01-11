namespace ToSic.Sxc.Images.Internal;

// TODO: LOCATION / NAMESPACE not final
[PrivateApi("WIP v16.08")]
public class CopyrightSettings(IEntity entity) : EntityBasedType(entity)
{
    public static string TypeNameId = "aed871cf-220b-4330-b368-f1259981c9c8";
    public static string NiceTypeName = "⚙️CopyrightSettings";

    // todo: unclear if nullable bools work, probably never tested before
    public bool? ImagesInputEnabled => GetThis(null as bool?);

}