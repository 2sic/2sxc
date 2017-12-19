using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using static System.String;

namespace ToSic.SexyContent.Internal
{
    internal class AppManagement
    {
        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <returns></returns>
        public static List<App> GetApps(int zoneId, bool includeDefaultApp, PortalSettings ownerPs, Log parentLog)
        {
            var appIds = new ZoneRuntime(zoneId, parentLog).Apps;
            var builtApps = appIds.Select(eavApp => new App(zoneId, eavApp.Key, ownerPs));

            if (!includeDefaultApp)
                builtApps = builtApps.Where(a => a.Name != Eav.Constants.ContentAppName);

            return builtApps.OrderBy(a => a.Name).ToList();
        }

        ///// <summary>
        ///// Create app-describing entity for configuration and add Settings and Resources Content Type
        ///// </summary>
        //internal static void EnsureAppIsConfigured(int zoneId, int appId, Log parentLog, string appName = null)
        //{
        //    var appAssignment = SystemRuntime.MetadataType(Eav.Constants.AppAssignmentName);
        //    var scope = Settings.AttributeSetScopeApps;
        //    var mds = DataSource.GetMetaDataSource(zoneId, appId);
        //    var appMetaData = mds.GetMetadata(appAssignment, appId, AppConstants.AttributeSetStaticNameApps).FirstOrDefault();
        //    var appResources = mds.GetMetadata(appAssignment, appId, AppConstants.AttributeSetStaticNameAppResources).FirstOrDefault();
        //    var appSettings = mds.GetMetadata(appAssignment, appId, AppConstants.AttributeSetStaticNameAppSettings).FirstOrDefault();

        //    // Get appName from cache - stop if it's a "Default" app
        //    var eavAppName = new ZoneRuntime(zoneId, parentLog).GetName(appId);

        //    if (eavAppName == Eav.Constants.DefaultAppName)
        //        return;

        //    var appMan = new AppManager(zoneId, appId);
        //    if (appMetaData == null)
        //        appMan.MetadataEnsureTypeAndSingleEntity(scope,
        //            AppConstants.AttributeSetStaticNameApps,
        //            "App Metadata",
        //            appAssignment,
        //            new Dictionary<string, object>()
        //            {
        //                {"DisplayName", IsNullOrEmpty(appName) ? eavAppName : appName},
        //                {"Folder", IsNullOrEmpty(appName) ? eavAppName : RemoveIllegalCharsFromPath(appName)},
        //                {"AllowTokenTemplates", "False"},
        //                {"AllowRazorTemplates", "False"},
        //                {"Version", "00.00.01"},
        //                {"OriginalId", ""}
        //            });


        //    // Add new (empty) ContentType for Settings
        //    if (appSettings == null)
        //        appMan.MetadataEnsureTypeAndSingleEntity(scope,
        //            AppConstants.AttributeSetStaticNameAppSettings,
        //            "Stores settings for an app",
        //            appAssignment, 
        //            null);
            
        //    // add new (empty) ContentType for Resources
        //    if (appResources == null)
        //        appMan.MetadataEnsureTypeAndSingleEntity(scope,
        //            AppConstants.AttributeSetStaticNameAppResources,
        //            "Stores resources like translations for an app", 
        //            appAssignment, 
        //            null);

        //    if (appMetaData == null || appSettings == null || appResources == null)
        //        SystemManager.Purge(zoneId, appId);
        //}

        ///// <summary>
        ///// Will create a new app in the system and initialize the basic settings incl. the 
        ///// app-definition
        ///// </summary>
        ///// <returns></returns>
        //internal static void AddBrandNewApp(int zoneId, string appName, Log parentLog)
        //{
        //    // check if invalid app-name
        //    if (appName == Eav.Constants.ContentAppName || appName == Eav.Constants.DefaultAppName || IsNullOrEmpty(appName) || !Regex.IsMatch(appName, "^[0-9A-Za-z -_]+$"))
        //        throw new ArgumentOutOfRangeException("appName '" + appName + "' not allowed");

        //    var appId = new ZoneManager(zoneId, parentLog).CreateApp();
        //    AppManager.EnsureAppIsConfigured(zoneId, appId, parentLog, appName);
        //}

        internal static void RemoveAppInDnnAndEav(IEnvironment<PortalSettings> env, int zoneId, int appId, PortalSettings ps, int userId, Log parentLog)
        {
            // check portal assignment and that it's not the default app
            if (zoneId != env.ZoneMapper.GetZoneId(ps.PortalId))
                throw new Exception("This app does not belong to portal " + ps.PortalId);

            if (appId == new ZoneRuntime(zoneId, parentLog).DefaultAppId)
                throw new Exception("The default app of a zone cannot be removed.");

            // Delete folder in dnn
            var sexyApp = new App(zoneId, appId, ps);
            if (!IsNullOrEmpty(sexyApp.Folder) && Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            new ZoneManager(zoneId, parentLog).DeleteApp(appId);
        }


        //private static string RemoveIllegalCharsFromPath(string path)
        //{
        //    var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        //    var r = new Regex($"[{Regex.Escape(regexSearch)}]");
        //    return r.Replace(path, "");
        //}
    }
}