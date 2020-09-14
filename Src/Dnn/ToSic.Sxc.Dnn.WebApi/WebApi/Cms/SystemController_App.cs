using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi.App;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class SystemController
    {
        [HttpGet]
        public List<AppDto> Apps(int zoneId) 
            => new AppsBackend().Init(Log).Apps(new DnnTenant().Init(ActiveModule.OwnerPortalID), GetBlock(), zoneId);

        [HttpGet]
        public void DeleteApp(int zoneId, int appId) 
            => new CmsZones(zoneId, Log).AppsMan.RemoveAppInTenantAndEav(appId);

        [HttpPost]
        public void App(int zoneId, string name) 
            => AppManager.AddBrandNewApp(zoneId, name, Log);

        [HttpGet]
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            SystemManager.Purge(zoneId, appId, log: Log);
            return wrapLog("ok", true);
        }
    }
}
