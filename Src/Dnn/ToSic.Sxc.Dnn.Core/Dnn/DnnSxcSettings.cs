using ToSic.Eav;

namespace ToSic.Sxc.Dnn
{
    internal class DnnSxcSettings
    {

        // Special user groups to determine if admins can really admin

        public static readonly string DnnGroupSxcDesigners = "2sxc designers";

        //public const string DnnGroupSxcDesigners = "2sxcDesigners";
        public static readonly string DnnGroupSxcAdmins = "2sxcAdministrators"; // New in v13.03 - to replace the old name


        public static class Installation
        {
            // This list is just used to run code-upgrades in DNN
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
                "15.02.00",
                // TODO: ADD 16.00.00 once the real version is bigger than that
                EavSystemInfo.VersionString,
            };

            // this is the last version which must run server-side change-code
            // it's not sql-code, as the SqlDataProvider files are imported by DNN, not by our code
            internal const string LastVersionWithServerChanges = "15.02.00";
            internal const string LastVersionWithDnnDbChanges = "15.02.00"; // just fyi, not used anywhere
        }
    }
}
