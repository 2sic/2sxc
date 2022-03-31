using System;
using System.Reflection;
using ToSic.Eav;

namespace ToSic.Sxc
{
    public partial class Settings
    {
        // Version is used also as cache-break for js assets.
        // In past build revision was good cache-break value, but since assemblies are deterministic 
        // we use application start unix time as slow changing revision value for cache-break purpose. 
        //public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly Version Version =
            VersionWithFakeBuildNumber(Assembly.GetExecutingAssembly().GetName().Version);

        /// <summary>
        /// application start unix time as slow changing revision value
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static Version VersionWithFakeBuildNumber(Version version) =>
            new Version(version.Major, version.Minor, version.Build,
                (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

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
                "08.11.00", // "08.12.00", 
                "09.00.00", // "09.00.01", "09.00.02", "09.01.00", /* note 09.01.01 was never released */ "09.01.02", "09.01.03", "09.02.00", "09.03.00", "09.03.01", "09.03.02", "09.03.03", "09.04.00", "09.04.01", "09.04.02", "09.04.03", "09.05.00", "09.05.01", "09.05.02",
                // "09.06.00", "09.06.01", "09.07.00", "09.08.00", "09.09.00", "09.10.00", "09.11.00", "09.11.01", "09.12.00", "09.13.00",
                // "09.14.00", // LTS
                // "09.20.00", "09.21.00", "09.22.00", "09.23.00", "09.30.00", "09.31.00","09.32.00", "09.33.00", "09.35.00", "09.40.00", "09.40.01", "09.41.00", "09.42.00", "09.43.00", /* LTS */ "09.43.01", /* LTS */
                "10.00.00", // "10.01.00", "10.02.00", "10.03.00", "10.04.00", "10.05.00", "10.06.00", "10.07.00", "10.08.00", "10.09.00",
                // "10.09.01", // LTS
                // "10.20.00", "10.20.01", "10.20.02", "10.20.03", "10.20.04", "10.20.05", "10.21.00", "10.22.00", "10.23.00",
                //"10.24.00", // LTS
                //"10.25.00", // LTS
                //"10.25.02", // LTS
                //"10.25.03",
                //"10.26.00", "10.27.00", "10.27.01", "10.28.00", "10.30.00",
                "11.00.00", // "11.01.00", "11.02.00", "11.03.00", "11.04.00", "11.05.00", "11.06.00", "11.06.01", "11.07.00",
                //"11.07.01", // LTS
                //"11.07.02", // LTS
                //"11.07.03", // LTS
                //"11.10.00", "11.10.01", "11.11.00", "11.11.01", "11.11.02", "11.11.03", "11.11.04", "11.12.00", "11.12.01", "11.20.00", "11.22.00",
                //"11.22.00", // LTS
                "12.00.00", // "12.01.00", "12.02.00", "12.02.01", "12.05.00",
                //"12.06.00", // LTS
                // "12.07.00", was never released
                //"12.08.00", "12.08.01", // LTS
                //"12.10.00",
                "13.00.00",
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
