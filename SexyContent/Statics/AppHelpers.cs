using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.DataSources.Caches;

namespace ToSic.SexyContent.Statics
{
    public class AppHelpers
    {
        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <param name="includeDefaultApp"></param>
        /// <returns></returns>
        public static List<App> GetApps(int zoneId, bool includeDefaultApp, PortalSettings ownerPS)
        {
            var eavApps = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps;
            var sexyApps = eavApps.Select<KeyValuePair<int, string>, App>(eavApp => GetApp(zoneId, eavApp.Key, ownerPS));

            if (!includeDefaultApp)
                sexyApps = sexyApps.Where(a => a.Name != "Content");

            return sexyApps.OrderBy(a => a.Name).ToList();
        }

        public static App GetApp(int zoneId, int appId, PortalSettings ownerPS)
        {
            // Get appName from cache
            var eavAppName = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];
            App sexyApp = null;

            if (eavAppName != Constants.DefaultAppName)
            {
                EnsureAppIsConfigured(zoneId, appId);

                // Get app-describing entity
                var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameApps).FirstOrDefault();
                var appResources = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameAppResources).FirstOrDefault();
                var appSettings = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameAppSettings).FirstOrDefault();

                if (appMetaData != null)
                {
                    dynamic appMetaDataDynamic = new DynamicEntity(appMetaData, new[] { Thread.CurrentThread.CurrentCulture.Name }, null);
                    dynamic appResourcesDynamic = appResources != null ? new DynamicEntity(appResources, new[] { Thread.CurrentThread.CurrentCulture.Name }, null) : null;
                    dynamic appSettingsDynamic = appResources != null ? new DynamicEntity(appSettings, new[] { Thread.CurrentThread.CurrentCulture.Name }, null) : null;

                    sexyApp = new App(appId, zoneId, ownerPS)
                    {
                        Name = appMetaDataDynamic.DisplayName,
                        Folder = appMetaDataDynamic.Folder,
                        Configuration = appMetaDataDynamic,
                        Resources = appResourcesDynamic,
                        Settings = appSettingsDynamic,
                        Hidden = appMetaDataDynamic.Hidden is bool ? appMetaDataDynamic.Hidden : false,
                        AppGuid = eavAppName
                    };
                }
            }
            // Handle default app
            else
            {
                sexyApp = new App(appId, zoneId, ownerPS)
                {
                    AppId = appId,
                    Name = "Content",
                    Folder = "Content",
                    Configuration = null,
                    Resources = null,
                    Settings = null,
                    Hidden = true,
                    AppGuid = eavAppName
                };
            }

            return sexyApp;
        }

        /// <summary>
        /// Create app-describing entity for configuration and add Settings and Resources Content Type
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        public static void EnsureAppIsConfigured(int zoneId, int appId, string appName = null)
        {
            var appMetaData = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameApps).FirstOrDefault();
            var appResources = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameAppResources).FirstOrDefault();
            var appSettings = DataSource.GetMetaDataSource(zoneId, appId).GetAssignedEntities(ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp, appId, Settings.AttributeSetStaticNameAppSettings).FirstOrDefault();

            // Get appName from cache
            var eavAppName = ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].Apps[appId];

            if (eavAppName == Constants.DefaultAppName)
                return;

            var appContext = new SexyContent(zoneId, appId);

            if (appMetaData == null)
            {
                // Add app-describing entity
                var appAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(Settings.AttributeSetStaticNameApps).AttributeSetID;
                var values = new OrderedDictionary
                {
                    {"DisplayName", String.IsNullOrEmpty(appName) ? eavAppName : appName },
                    {"Folder", String.IsNullOrEmpty(appName) ? eavAppName : RemoveIllegalCharsFromPath(appName) },
                    {"AllowTokenTemplates", "False"},
                    {"AllowRazorTemplates", "False"},
                    {"Version", "00.00.01"},
                    {"OriginalId", ""}
                };
                appContext.ContentContext.Entities.AddEntity(appAttributeSet, values, null, appId, ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp);
            }

            if (appSettings == null)
            {

                AttributeSet settingsAttributeSet;
                // Add new (empty) ContentType for Settings
                if (!appContext.ContentContext.AttribSet.AttributeSetExists(Settings.AttributeSetStaticNameAppSettings, appId))
                    settingsAttributeSet = appContext.ContentContext.AttribSet.AddAttributeSet(Settings.AttributeSetStaticNameAppSettings,
                        "Stores settings for an app", Settings.AttributeSetStaticNameAppSettings, Settings.AttributeSetScopeApps);
                else
                    settingsAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(Settings.AttributeSetStaticNameAppSettings);

                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.Entities.AddEntity(settingsAttributeSet, new OrderedDictionary(), null, appId, ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp);
            }

            if (appResources == null)
            {
                AttributeSet resourcesAttributeSet;

                // Add new (empty) ContentType for Resources
                if (!appContext.ContentContext.AttribSet.AttributeSetExists(Settings.AttributeSetStaticNameAppResources, appId))
                    resourcesAttributeSet = appContext.ContentContext.AttribSet.AddAttributeSet(
                        Settings.AttributeSetStaticNameAppResources, "Stores resources like translations for an app",
                        Settings.AttributeSetStaticNameAppResources, Settings.AttributeSetScopeApps);
                else
                    resourcesAttributeSet = appContext.ContentContext.AttribSet.GetAttributeSet(Settings.AttributeSetStaticNameAppResources);

                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
                appContext.ContentContext.Entities.AddEntity(resourcesAttributeSet, new OrderedDictionary(), null, appId, ContentTypeHelpers.AssignmentObjectTypeIDSexyContentApp);
            }

            if (appMetaData == null || appSettings == null || appResources == null)
                DataSource.GetCache(zoneId, appId).PurgeCache(zoneId, appId);
        }


        public static App AddApp(int zoneId, string appName, PortalSettings ownerPS)
        {
            if (appName == "Content" || appName == "Default" || String.IsNullOrEmpty(appName) || !Regex.IsMatch(appName, "^[0-9A-Za-z -_]+$"))
                throw new ArgumentOutOfRangeException("appName '" + appName + "' not allowed");

            // Adding app to EAV
            var sexy = new SexyContent(zoneId, AppHelpers.GetDefaultAppId(zoneId));
            var app = sexy.ContentContext.App.AddApp(Guid.NewGuid().ToString());
            sexy.ContentContext.SqlDb.SaveChanges();

            EnsureAppIsConfigured(zoneId, app.AppID, appName);

            return GetApp(zoneId, app.AppID, ownerPS);
        }

        public static void RemoveApp(int zoneId, int appId, PortalSettings ps, int userId)
        {
            if (zoneId != ZoneHelpers.GetZoneID(ps.PortalId))
                throw new Exception("This app does not belong to portal " + ps.PortalId);

            var sexy = new SexyContent(zoneId, appId, false);

            if (appId != sexy.ContentContext.AppId)
                throw new Exception("An app can only be removed inside of it's own context.");

            if (appId == AppHelpers.GetDefaultAppId(zoneId))
                throw new Exception("The default app of a zone cannot be removed.");

            var sexyApp = AppHelpers.GetApp(zoneId, appId, ps);

            // Delete folder
            if (!String.IsNullOrEmpty(sexyApp.Folder) && Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            // Delete the app
            sexy.ContentContext.App.DeleteApp(appId);
        }

        // 2016-02-26 2dm probably unused
        //public static int? GetAppSettingsAttributeSetId(int zoneId, int appId)
        //{
        //    if (appId == AppHelpers.GetDefaultAppId(zoneId))
        //        return null;

        //    return new SexyContent(zoneId, appId).GetAvailableContentTypes(SexyContent.AttributeSetScopeApps)
        //        .Single(p => p.StaticName == SexyContent.AttributeSetStaticNameAppSettings).AttributeSetId;
        //}

        // 2016-02-26 2dm probably unused
        //public static int? GetAppResourcesAttributeSetId(int zoneId, int appId)
        //{
        //    if (appId == AppHelpers.GetDefaultAppId(zoneId))
        //        return null;

        //    return new SexyContent(zoneId, appId).GetAvailableContentTypes(SexyContent.AttributeSetScopeApps)
        //        .Single(p => p.StaticName == SexyContent.AttributeSetStaticNameAppResources).AttributeSetId;
        //}

        public static int? GetAppIdFromModule(ModuleInfo module)
        {
            var zoneId = ZoneHelpers.GetZoneID(module.OwnerPortalID);

            if (module.DesktopModule.ModuleName == "2sxc")
            {
                return zoneId.HasValue ? AppHelpers.GetDefaultAppId(zoneId.Value) : new int?();
            }

            object appIdString = null;

            if (HttpContext.Current != null)
            {
                if (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["AppId"]))
                    appIdString = HttpContext.Current.Request.QueryString["AppId"];
                else
                {
                    // todo: fix 2dm
                    var appName = DnnStuffToRefactor.TryToGetReliableSetting(module, Settings.AppNameString);//  module.ModuleSettings[AppNameString];

                    if (appName != null)
                    {
                        // ToDo: Fix issue in EAV (cache is only ensured when a CacheItem-Property is accessed like LastRefresh)
                        var x = ((BaseCache)DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)).LastRefresh;
                        appIdString = ((BaseCache)DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)).ZoneApps[zoneId.Value].Apps.Where(p => p.Value == (string)appName).Select(p => p.Key).FirstOrDefault();
                    }
                }
            }

            int appId;
            if (appIdString != null && Int32.TryParse(appIdString.ToString().Split(',')[0], out appId))
                return appId;

            return null;
        }

        public static void SetAppIdForModule(ModuleInfo module, int? appId)
        {
            var moduleController = new ModuleController();

            // Reset temporary template
            ContentGroups.DeletePreviewTemplateId(module.ModuleID);

            // ToDo: Should throw exception if a real ContentGroup exists

            var zoneId = ZoneHelpers.GetZoneID(module.OwnerPortalID);

            if (appId == 0 || !appId.HasValue)
                //moduleController.DeleteModuleSetting(module.ModuleID, AppNameString);
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, null);
            else
            {
                var appName = ((BaseCache)DataSource.GetCache(0, 0)).ZoneApps[zoneId.Value].Apps[appId.Value];
                //moduleController.UpdateModuleSetting(module.ModuleID, AppNameString, appName);
                DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(module.ModuleID, Settings.AppNameString, appName);
            }

            // Change to 1. available template if app has been set
            if (appId.HasValue)
            {
                var sexyForNewApp = new SexyContent(zoneId.Value, appId.Value, false);
                var templates = sexyForNewApp.Templates.GetAvailableTemplatesForSelector(module.ModuleID, sexyForNewApp.ContentGroups).ToList();
                if (templates.Any())
                    sexyForNewApp.ContentGroups.SetPreviewTemplateId(module.ModuleID, templates.First().TemplateId);
            }
        }


        public static int GetDefaultAppId(int zoneId)
        {
            return ((BaseCache)DataSource.GetCache(zoneId, null)).ZoneApps[zoneId].DefaultAppId;
        }


        public static string AppBasePath(PortalSettings ownerPS)
        {
            return Path.Combine(ownerPS.HomeDirectory, Settings.TemplateFolder);
        }


        internal static string RemoveIllegalCharsFromPath(string path)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(String.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    }
}