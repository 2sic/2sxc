using ToSic.Eav.Sys;

namespace ToSic.Sxc.Dnn;

internal class DnnSxcSettings
{

    /// <summary>
    /// Special user groups to determine if admins can really admin
    /// </summary>
    public static readonly string[] DnnGroupsSxcAdmins =
    [
        "2sxc designers",       // Old, original name since ca. 2sxc 4+
        "2sxcAdministrators",   // New in v13.03 - to replace the old name for clarity
    ];

    public const string DnnAdminRoleDefaultName = "Administrators";

    /// <summary>
    /// Content Editors Advanced Role Name.
    /// </summary>
    /// <remarks>
    /// AdvancedPermissionProvider in DNN v10
    /// </remarks>
    public const string DnnContentEditors = "Content Editors";

    /// <summary>
    /// Content Managers Advanced Role Name.
    /// </summary>
    /// <remarks>
    /// AdvancedPermissionProvider in DNN v10
    /// </remarks>
    public const string DnnContentManagers = "Content Managers";

    public static class Installation
    {
        // This list is just used to run code-upgrades in DNN
        // So we only need the versions which do have code upgrades - which is very uncommon
        // todo: Maybe this list can somehow be extracted from the module manifest or placed there...
        internal static readonly string[] UpgradeVersionList =
        [
            // new installer of 2sxc v20 doesn't upgrade versions before v15, so removed all those versions
            "15.00.00",
            "15.02.00",
            "20.00.00",
            "20.00.01",
            EavSystemInfo.VersionString,
        ];

        // this is the last version which must run server-side change-code
        // it's not sql-code, as the SqlDataProvider files are imported by DNN, not by our code
        internal const string LastVersionWithServerChanges = "20.00.01";
        internal const string LastVersionWithDnnDbChanges = "21.00.00"; // just fyi, not used anywhere
    }
}