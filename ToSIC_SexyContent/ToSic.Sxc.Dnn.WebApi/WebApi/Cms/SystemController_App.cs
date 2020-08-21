using System.Linq;
using System.Web.Http;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class SystemController
    {
        [HttpGet]
        public dynamic Apps(int zoneId)
        {
            var cms = new CmsZones(zoneId, Env, Log);
            var tenant = new DnnTenant().Init(ActiveModule.OwnerPortalID);
            var configurationBuilder = ConfigurationProvider.Build(BlockBuilder, true);
            var list = cms.AppsRt.GetApps(tenant, configurationBuilder);
            return list.Select(a => new
            {
                Id = a.AppId,
                IsApp = a.AppGuid != Eav.Constants.DefaultAppName,
                Guid = a.AppGuid,
                a.Name,
                a.Folder,
                AppRoot = a.Path,
                IsHidden = a.Hidden,
                ConfigurationId = a.Configuration?.Id,
                Items = a.Data.List.Count(),
                a.Thumbnail,
                Version = a.VersionSafe()
            }).ToList();
        }

        [HttpGet]
        public void DeleteApp(int zoneId, int appId) => new CmsZones(zoneId, Env, Log).AppsMan.RemoveAppInTenantAndEav(appId);

        [HttpPost]
        public void App(int zoneId, string name) => AppManager.AddBrandNewApp(zoneId, name, Log);

        [HttpGet]
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            SystemManager.Purge(zoneId, appId, log: Log);
            return wrapLog("ok", true);
        }
    }
}
