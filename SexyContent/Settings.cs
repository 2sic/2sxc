using System;
using System.Reflection;
using ToSic.Eav;

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
        public static readonly string InternalUserName = "Internal";


        public static readonly string PortalHostDirectory = "~/Portals/_default/";
        public static readonly string TemplateFolder = "2sxc";
        public static readonly string PortalSettingsPrefix = "ToSIC_SexyContent_";
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
        public static readonly string ToSexyDirectory = "~/DesktopModules/ToSIC_SexyContent";
        public static readonly string TemporaryDirectory = "~/DesktopModules/ToSIC_SexyContent/_";

        /// <summary>
        /// Collection of Template Locations
        /// </summary>
        public class TemplateLocations
        {
            public const string PortalFileSystem = "Portal File System";
            public const string HostFileSystem = "Host File System";
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
            new UnityConfig().Configure();
            SetEavConnectionString();
        }

        public static void SetEavConnectionString()
        {
            Configuration.SetConnectionString("SiteSqlServer");
        }

        #endregion
    }
}