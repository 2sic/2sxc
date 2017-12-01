using System;
using System.Configuration;
using System.Reflection;
using Configuration = ToSic.Eav.Configuration;

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

        public static readonly string PortalHostDirectory = "~/Portals/_default/";
        public static readonly string TemplateFolder = "2sxc";
        public const string PortalSettingsPrefix = "ToSIC_SexyContent_";
        public const string PortalSettingZoneId = PortalSettingsPrefix + "ZoneID";
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static readonly string ModuleVersion = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("00");

        public static readonly string WebConfigTemplatePath = "~/DesktopModules/ToSIC_SexyContent/WebConfigTemplate.config";
        public static readonly string WebConfigFileName = "web.config";
        public static readonly string SexyContentGroupName = "2sxc designers";
        public static readonly string AttributeSetScope = "2SexyContent";
        public static readonly string AttributeSetScopeApps = "2SexyContent-App";
        public static readonly string AttributeSetStaticNameTemplateMetaData = "2SexyContent-Template-Metadata";
        public static readonly string AttributeSetStaticNameApps = "2SexyContent-App";
        public static readonly string AttributeSetStaticNameAppResources = "App-Resources";
        public static readonly string AttributeSetStaticNameAppSettings = "App-Settings";
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
                "07.00.00", "07.00.03", "07.02.00", "07.02.02", "07.03.00", "07.03.01", "07.03.02", "07.03.03", "07.03.04",
                "08.00.00", "08.00.01", "08.00.02", "08.00.03", "08.00.04", "08.00.05", "08.00.06", "08.00.07", "08.00.08", "08.00.09", "08.00.10", "08.00.11", "08.00.12",
                "08.01.00", "08.01.01", "08.01.02", "08.01.03", "08.02.00", "08.02.01", "08.02.02", "08.02.03",
                "08.03.00", "08.03.01", "08.03.02", "08.03.03", "08.03.04", "08.03.05", "08.03.06", "08.03.07", "08.04.00", "08.04.01", "08.04.02", "08.04.03", "08.04.04", "08.04.05", "08.04.06", "08.04.07", "08.04.08",
                "08.05.00", "08.05.01", "08.05.02", "08.05.03", "08.05.04", "08.05.05", "08.05.06", "08.06.00", "08.07.00", "08.08.00", "08.08.01", "08.08.02","08.09.00","08.09.01","08.10.00","08.10.01",
                // new installer starting in 08.11.00!
                "08.11.00", "08.12.00", "09.00.00", "09.00.01", "09.00.02", "09.01.00", /* note 09.01.01 was never released */ "09.01.02", "09.01.03", "09.02.00", "09.03.00", "09.03.01", "09.03.02", "09.03.03", "09.04.00", "09.04.01", "09.04.02", "09.04.03", "09.05.00", "09.05.01", "09.05.02",
                "09.06.00", "09.06.01", "09.07.00", "09.08.00", "09.09.00", "09.10.00", "09.11.00"
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
        static Settings()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            Configuration.SetConnectionString(connectionString);
            new UnityConfig().Configure();
        }

        #endregion
    }
}