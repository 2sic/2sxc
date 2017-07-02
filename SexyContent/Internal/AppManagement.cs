using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    internal class AppManagement
    {
        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="includeDefaultApp"></param>
        /// <param name="ownerPs"></param>
        /// <returns></returns>
        public static List<App> GetApps(int zoneId, bool includeDefaultApp, PortalSettings ownerPs)
        {
            var appIds = new ZoneRuntime(zoneId).Apps;// State.GetAppList(zoneId);
            var sexyApps = appIds.Select(eavApp => new App(zoneId, eavApp.Key, ownerPs));

            if (!includeDefaultApp)
                sexyApps = sexyApps.Where(a => a.Name != Constants.ContentAppName);

            return sexyApps.OrderBy(a => a.Name).ToList();
        }

        /// <summary>
        /// Create app-describing entity for configuration and add Settings and Resources Content Type
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        internal static void EnsureAppIsConfigured(int zoneId, int appId, string appName = null)
        {
            var appAssignment = SystemRuntime.GetMetadataType(Constants.AppAssignmentName);
            var scope = Settings.AttributeSetScopeApps;
            var mds = DataSource.GetMetaDataSource(zoneId, appId);
            var appMetaData = mds.GetAssignedEntities(appAssignment, appId, Settings.AttributeSetStaticNameApps).FirstOrDefault();
            var appResources = mds.GetAssignedEntities(appAssignment, appId, Settings.AttributeSetStaticNameAppResources).FirstOrDefault();
            var appSettings = mds.GetAssignedEntities(appAssignment, appId, Settings.AttributeSetStaticNameAppSettings).FirstOrDefault();

            // Get appName from cache - stop if it's a "Default" app
            var eavAppName = new ZoneRuntime(zoneId).GetName(appId);// State.GetAppName(zoneId, appId); 

            if (eavAppName == Eav.Constants.DefaultAppName)
                return;

            var appMan = new AppManager(zoneId, appId);
            if (appMetaData == null)
                appMan.MetadataEnsureTypeAndSingleEntity(scope,
                    Settings.AttributeSetStaticNameApps,
                    "App Metadata",
                    appAssignment,
                    new Dictionary<string, object>()
                    {
                        {"DisplayName", IsNullOrEmpty(appName) ? eavAppName : appName},
                        {"Folder", IsNullOrEmpty(appName) ? eavAppName : RemoveIllegalCharsFromPath(appName)},
                        {"AllowTokenTemplates", "False"},
                        {"AllowRazorTemplates", "False"},
                        {"Version", "00.00.01"},
                        {"OriginalId", ""}
                    });


            // Add new (empty) ContentType for Settings
            if (appSettings == null)
                appMan.MetadataEnsureTypeAndSingleEntity(scope,
                    Settings.AttributeSetStaticNameAppSettings,
                    "Stores settings for an app",
                    appAssignment, 
                    null);
            
            // add new (empty) ContentType for Resources
            if (appResources == null)
                appMan.MetadataEnsureTypeAndSingleEntity(scope,
                    Settings.AttributeSetStaticNameAppResources,
                    "Stores resources like translations for an app", 
                    appAssignment, 
                    null);

            if (appMetaData == null || appSettings == null || appResources == null)
                SystemManager.Purge(zoneId, appId);
        }

        /// <summary>
        /// Will create a new app in the system and initialize the basic settings incl. the 
        /// app-definition
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appName"></param>
        /// <param name="ownerPs"></param>
        /// <returns></returns>
        internal static void AddBrandNewApp(int zoneId, string appName, PortalSettings ownerPs)
        {
            // check if invalid app-name
            if (appName == Constants.ContentAppName || appName == Eav.Constants.DefaultAppName || IsNullOrEmpty(appName) || !Regex.IsMatch(appName, "^[0-9A-Za-z -_]+$"))
                throw new ArgumentOutOfRangeException("appName '" + appName + "' not allowed");

            var appId = new ZoneManager(zoneId).CreateApp();// State.AppCreate(zoneId);

            EnsureAppIsConfigured(zoneId, appId, appName);
        }

        internal static void RemoveAppInDnnAndEav(int zoneId, int appId, PortalSettings ps, int userId)
        {
            // check portal assignment and that it's not the default app
            if (zoneId != new Environment.DnnEnvironment().ZoneMapper.GetZoneId(ps.PortalId))//  ZoneHelpers.GetZoneId(ps.PortalId) )
                throw new Exception("This app does not belong to portal " + ps.PortalId);

            if (appId == new ZoneRuntime(zoneId).DefaultAppId)// State.GetDefaultAppId(zoneId))
                throw new Exception("The default app of a zone cannot be removed.");

            // Delete folder in dnn
            var sexyApp = new App(zoneId, appId, ps);
            if (!IsNullOrEmpty(sexyApp.Folder) && Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            new ZoneManager(zoneId).DeleteApp(appId); //State.AppDelete(zoneId, appId);
        }


        private static string RemoveIllegalCharsFromPath(string path)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(path, "");
        }
    }
}