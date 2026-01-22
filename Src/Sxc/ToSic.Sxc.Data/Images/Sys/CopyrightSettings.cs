using ToSic.Eav.Model;

namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// Internal object to read image copyright metadata.
/// </summary>
/// <remarks>
/// Created ca. v16.08
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public record CopyrightSettings : ModelOfEntityCore
{
    public static string TypeNameId = "aed871cf-220b-4330-b368-f1259981c9c8";
    public static string NiceTypeName = "⚙️CopyrightSettings";

    public CopyrightSettings(IEntity entity) : base(entity) { }

    // todo: unclear if nullable booleans work, probably never tested before
    public bool? ImagesInputEnabled => GetThis(null as bool?);

}