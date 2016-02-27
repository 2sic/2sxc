using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ToSic.SexyContent
{
    public class Settings
    {
        public const string ContentGroupGuidString = "ToSIC_SexyContent_ContentGroupGuid";
        public const string AppIDString = "AppId";
        public const string AppNameString = "ToSIC_SexyContent_AppName";
        public const string SettingsShowTemplateChooser = "ToSIC_SexyContent_ShowTemplateChooser";
        public const string InternalUserName = "Internal";


        public const string PortalHostDirectory = "~/Portals/_default/";
        public const string TemplateFolder = "2sxc";
        public const string PortalSettingsPrefix = "ToSIC_SexyContent_";
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static readonly string ModuleVersion = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString("00") + "."
                                                      + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString("00");

        public const string WebConfigTemplatePath = "~/DesktopModules/ToSIC_SexyContent/WebConfigTemplate.config";
        public const string WebConfigFileName = "web.config";
        public const string SexyContentGroupName = "2sxc designers";
        public const string AttributeSetScope = "2SexyContent";
        public const string AttributeSetScopeApps = "2SexyContent-App";
        public const string AttributeSetStaticNameTemplateMetaData = "2SexyContent-Template-Metadata";
        public const string AttributeSetStaticNameApps = "2SexyContent-App";
        public const string AttributeSetStaticNameAppResources = "App-Resources";
        public const string AttributeSetStaticNameAppSettings = "App-Settings";
        public const string ToSexyDirectory = "~/DesktopModules/ToSIC_SexyContent";
        public const string TemporaryDirectory = "~/DesktopModules/ToSIC_SexyContent/_";

        /// <summary>
        /// Collection of Template Locations
        /// </summary>
        public class TemplateLocations
        {
            public const string PortalFileSystem = "Portal File System";
            public const string HostFileSystem = "Host File System";
        }
    }
}