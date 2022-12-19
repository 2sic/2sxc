using ToSic.Eav;

namespace ToSic.Sxc
{
    public partial class Settings
    {
        public const string WebConfigTemplateFile = "WebConfigTemplate.config";
        public const string WebConfigFileName = "web.config";

        // Special user groups to determine if admins can really admin
        public const string DnnGroupSxcDesigners = "2sxc designers";
        //public const string DnnGroupSxcDesigners = "2sxcDesigners";
        public const string DnnGroupSxcAdmins = "2sxcAdministrators"; // New in v13.03 - to replace the old name

        public class Installation
        {
            // This list is just used to run code-upgrades
            // So we only need the versions which do have code upgrades - which is very uncommon
            // todo: Maybe this list can somehow be extracted from the module manifest or placed there...
            internal static readonly string[] UpgradeVersionList =
            {
                // new installer of 2sxc 9.20 doesn't upgrade versions before 8.12, so removed all those versions
                "08.11.00",
                "09.00.00", 
                "10.00.00", 
                "11.00.00", 
                "12.00.00",
                "13.00.00",
                "15.00.00",
                // 13.01.00
                EavSystemInfo.VersionString,
            };

            // this is the last version which must run server-side change-code
            // it's not sql-code, as the SqlDataProvider files are imported by DNN, not by our code
            internal const string LastVersionWithServerChanges = "08.11.00";
            internal const string LastVersionWithDnnDbChanges = "13.04.00"; // just fyi, not used anywhere
        }
    }
}
