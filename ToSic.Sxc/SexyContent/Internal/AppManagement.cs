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
        public static List<App> GetApps(int zoneId, bool includeDefaultApp, /*PortalSettings ownerPs*/ ITennant tennant, Log parentLog)
        {
            var appIds = new ZoneRuntime(zoneId, parentLog).Apps;
            var builtApps = appIds.Select(eavApp => new App(zoneId, eavApp.Key, tennant));

            if (!includeDefaultApp)
                builtApps = builtApps.Where(a => a.Name != Eav.Constants.ContentAppName);

            return builtApps.OrderBy(a => a.Name).ToList();
        }

        internal static void RemoveAppInDnnAndEav(IEnvironment<PortalSettings> env, int zoneId, int appId, ITennant tennant /*PortalSettings ps*/, int userId, Log parentLog)
        {
            // check portal assignment and that it's not the default app
            if (zoneId != env.ZoneMapper.GetZoneId(/*ps.PortalId*/tennant.Id))
                throw new Exception("This app does not belong to portal " + /*ps.PortalId*/tennant.Id);

            if (appId == new ZoneRuntime(zoneId, parentLog).DefaultAppId)
                throw new Exception("The default app of a zone cannot be removed.");

            // Delete folder in dnn
            var sexyApp = new App(zoneId, appId, /*ps*/tennant);
            if (!IsNullOrEmpty(sexyApp.Folder) && Directory.Exists(sexyApp.PhysicalPath))
                Directory.Delete(sexyApp.PhysicalPath, true);

            new ZoneManager(zoneId, parentLog).DeleteApp(appId);
        }

    }
}