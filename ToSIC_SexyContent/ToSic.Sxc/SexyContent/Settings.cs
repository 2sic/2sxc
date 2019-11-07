using System;
using System.Reflection;
using ToSic.Eav.Plumbing.Booting;
 
namespace ToSic.SexyContent
{
    public class Settings
    {
        // Important note: always use static-readonly, NOT constant
        // reason is that we must ensure that the static constructor is called 
        // whenever anything is accessed
        public static readonly string ContentGroupGuidString = "ToSIC_SexyContent_ContentGroupGuid";
        public static readonly string AppIDString = "AppId";
        public static readonly string AppNameString = "ToSIC_SexyContent_AppName";
        public static readonly string SettingsShowTemplateChooser = "ToSIC_SexyContent_ShowTemplateChooser";
        public static readonly string PreviewTemplateIdString = "ToSIC_SexyContent_PreviewTemplateId";
        internal static readonly string InternalUserName = "Internal";

        //internal static readonly string ContentAppName = "Default";

        public const string PortalHostDirectory = "~/Portals/_default/";
        public const string AppsRootFolder = "2sxc";
        public const string PortalSettingsPrefix = "ToSIC_SexyContent_";
        public const string PortalSettingZoneId = PortalSettingsPrefix + "ZoneID";
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static readonly string ModuleVersion = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("00");

        public const string WebConfigTemplatePath = "~/DesktopModules/ToSIC_SexyContent/WebConfigTemplate.config";
        public const string WebConfigFileName = "web.config";
        public const string SexyContentGroupName = "2sxc designers";
        public const string AttributeSetScope = "2SexyContent";

        internal static readonly string AttributeSetStaticNameContentBlockTypeName = "ContentGroupReference";

        internal static readonly int DataIsMissingInDb = -100;

        /// <summary>
        /// Collection of Template Locations
        /// </summary>
        public class TemplateLocations
        {
            public const string PortalFileSystem = "Portal File System";
            public const string HostFileSystem = "Host File System";
        }

        public class Installation
        {
            internal const string LogDirectory = "~/DesktopModules/ToSIC_SexyContent/Upgrade/Log/";

            // todo: Maybe this list can somehow be extracted from the module manifest or placed there...
            internal static readonly string[] UpgradeVersionList =
            {
                // new installer of 2sxc 9.20 doesn't upgrade versions before 8.12, so removed all those versions
                "08.11.00", "08.12.00", "09.00.00", "09.00.01", "09.00.02", "09.01.00", /* note 09.01.01 was never released */ "09.01.02", "09.01.03", "09.02.00", "09.03.00", "09.03.01", "09.03.02", "09.03.03", "09.04.00", "09.04.01", "09.04.02", "09.04.03", "09.05.00", "09.05.01", "09.05.02",
                "09.06.00", "09.06.01", "09.07.00", "09.08.00", "09.09.00", "09.10.00", "09.11.00", "09.11.01", "09.12.00", "09.13.00",
                "09.14.00", /* LTS */
                "09.20.00", "09.21.00", "09.22.00", "09.23.00", "09.30.00", "09.31.00","09.32.00", "09.33.00", "09.35.00", "09.40.00", "09.40.01", "09.41.00", "09.42.00", "09.43.00", /* LTS */ "09.43.01", /* LTS */
                "10.00.00", "10.01.00", "10.02.00", "10.03.00", "10.04.00", "10.05.00", "10.06.00", "10.07.00", "10.08.00", 
                "10.09.00", "10.09.01", /* LTS */
                "10.20.00"
            };

            // this is the last version which must run server-side change-code
            // it's not sql-code, as the SqlDataProvider files are imported by DNN, not by our code
            internal const string LastVersionWithServerChanges = "08.11.00";
        }

        #region System Initialization

        /// <summary>
        /// This is a placeholder method, please call it in code places where you must ensure that
        /// everything is initialized, especially the EAV etc. 
        /// Reason is that once this object is accessed statically, the (other) static initializer will be called
        /// which will take care of all registration
        /// 
        /// This is a minor workaround, I'll try to find a cleaner way to do this some other time
        /// Before it happened automatically, because we had all the constants as part of this object...
        /// </summary>
        public static void EnsureSystemIsInitialized() { } 


        /// <summary>
        /// This is needed so when the application starts, we can configure our IoC container
        /// It is automatically executed when the first variable on this class (contstant, static, etc.)
        /// is accessed. 
        /// </summary>
        static Settings() => Boot.RunBootSequence();

        #endregion
    }
}