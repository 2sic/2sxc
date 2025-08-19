namespace ToSic.Sxc.Cms;

/// <summary>
/// Contains special constants for setting-names stored in the Dnn/Oqtane site settings.
/// </summary>
/// <remarks>
/// Note that for historical reasons, the keys where different in Dnn and Oqtane until 2025-08-19.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SiteSettingNames
{
    // Important note: always use static-readonly, NOT constant
    // This prevents the value from being compiled into other DLLs,
    // So if a value ever changes, it will always be retrieved from here

    /// <summary>
    /// This site setting pointing to the zone of site.
    /// The value must contain the int.
    /// </summary>
    /// <remarks>
    /// "EavZone" in Oqtane until 2025-08-19
    /// </remarks>
    public static readonly string SiteKeyForZoneId = "TsDynDataZoneId";

}