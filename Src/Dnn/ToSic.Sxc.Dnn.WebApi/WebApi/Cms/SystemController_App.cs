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
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            SystemManager.Purge(zoneId, appId, log: Log);
            return wrapLog("ok", true);
        }
    }
}
