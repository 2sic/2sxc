namespace ToSic.Sxc.Images.Sys;

/// <summary>
/// System Settings regarding Copyright Configuration of an App/Site
/// </summary>
/// <remarks>
/// * Created ca. v16.08
/// * As of v21 not actively used in code, but in this structure to ensure consistency
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ModelSource(ContentType = ContentTypeNameId)]
public record CopyrightSettings : ModelOfEntityCore
{
    /// <summary>
    /// Where these settings are placed in the Settings structure - for use as prefix when accessing settings.
    /// </summary>
    /// <remarks>
    /// To be used like `SettingsPath + "." + KeyName`
    /// </remarks>
    public const string SettingsPath = "Copyright";

    public const string ContentTypeNameId = "aed871cf-220b-4330-b368-f1259981c9c8";
    public const string ContentTypeName = "⚙️CopyrightSettings";

    // todo: unclear if nullable booleans work, probably never tested before
    public bool? ImagesInputEnabled => GetThis(null as bool?);

}