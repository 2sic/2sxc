using ToSic.Eav.Data.Sys.Entities;

namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// Internal object to read image copyright metadata.
/// </summary>
/// <remarks>
/// Created ca. v16.08
/// </remarks>
[method: PrivateApi]
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class CopyrightSettings(IEntity entity) : EntityBasedType(entity)
{
    public static string TypeNameId = "aed871cf-220b-4330-b368-f1259981c9c8";
    public static string NiceTypeName = "⚙️CopyrightSettings";

    // todo: unclear if nullable booleans work, probably never tested before
    public bool? ImagesInputEnabled => GetThis(null as bool?);

}