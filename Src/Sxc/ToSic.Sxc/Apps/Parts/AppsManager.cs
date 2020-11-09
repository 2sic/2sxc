using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class AppsManager: ZonePartRuntimeBase<ZoneRuntime, AppsManager>
    {
        #region Constructor / DI

        public AppsManager(Lazy<ZoneManager> zoneManagerLazy, IServiceProvider serviceProvider) : base(serviceProvider, "Cms.AppsRt")
        {
            _zoneManagerLazy = zoneManagerLazy;
        }
        private readonly Lazy<ZoneManager> _zoneManagerLazy;

        #endregion


        internal void RemoveAppInSiteAndEav(int appId)
        {
            var zoneId = ZoneRuntime.ZoneId;
            // check portal assignment and that it's not the default app
            //if (zoneId != CmsZones.Env.ZoneMapper.GetZoneId(tenant.Id))
            //    throw new Exception("This app does not belong to portal " + tenant.Id);

            if (appId == ZoneRuntime.DefaultAppId)
                throw new Exception("The default app of a zone cannot be removed.");

            // todo: maybe verify the app is of this portal; I assume delete will fail anyhow otherwise

            // Prepare to Delete folder in dnn - this must be done, before deleting the app in the DB
            var sexyApp = ServiceProvider.Build<App>().InitNoData(new AppIdentity(zoneId, appId), null);
            var folder = sexyApp.Folder;
            var physPath = sexyApp.PhysicalPath;

            // now remove from DB. This sometimes fails, so we do this before trying to clean the files
            // as the db part should be in a transaction, and if it fails, everything should stay as is
            _zoneManagerLazy.Value.Init(zoneId, Log).DeleteApp(appId);

            // now really delete the files - if the DB didn't end up throwing an error
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(physPath))
                Directory.Delete(physPath, true);

        }
    }
}
