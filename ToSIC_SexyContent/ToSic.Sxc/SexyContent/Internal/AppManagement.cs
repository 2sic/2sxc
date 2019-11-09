using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.SxcTemp;
using static System.String;
using IApp = ToSic.Sxc.Apps.IApp;


// todo: move to eav
namespace ToSic.SexyContent.Internal
{
    internal class AppManagement
    {
        /// <summary>
        /// Returns all Apps for the current zone
        /// </summary>
        /// <returns></returns>
        public static List<IApp> GetApps(int zoneId, bool includeDefaultApp, ITenant tenant, ILog parentLog)
        {
            var appIds = new ZoneRuntime(zoneId, parentLog).Apps;
            var builtApps = appIds.Select(eavApp => GetApp.LightWithoutData(tenant, zoneId, eavApp.Key, parentLog: parentLog));

            if (!includeDefaultApp)
                builtApps = builtApps.Where(a => a.Name != Eav.Constants.ContentAppName);

            return builtApps.OrderBy(a => a.Name).ToList();
        }

        internal static void RemoveAppInTenantAndEav(IZoneMapper zoneMapper, int zoneId, int appId, ITenant tenant, int userId, ILog parentLog)
        {
            // check portal assignment and that it's not the default app
            if (zoneId != zoneMapper.GetZoneId(tenant.Id))
                throw new Exception("This app does not belong to portal " + tenant.Id);

            if (appId == new ZoneRuntime(zoneId, parentLog).DefaultAppId)
                throw new Exception("The default app of a zone cannot be removed.");

            // Prepare to Delete folder in dnn - this must be done, before deleting the app in the DB
            var sexyApp = GetApp.LightWithoutData(tenant, zoneId, appId, null);
            var folder = sexyApp.Folder;
            var physPath = sexyApp.PhysicalPath;

            // now remove from DB. This sometimes fails, so we do this before trying to clean the files
            // as the db part should be in a transaction, and if it fails, everything should stay as is
            new ZoneManager(zoneId, parentLog).DeleteApp(appId);

            // now really delete the files - if the DB didn't end up throwing an error
            if (!IsNullOrEmpty(folder) && Directory.Exists(physPath))
                Directory.Delete(physPath, true);

        }

    }
}