using System.Web.Http;
using ToSic.Eav.Apps;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Purge(int? appId = null)
        {
            ThrowIfNotSuperuser();

            if (appId == null)
                return "please add appid to the url parameters";

            SystemManager.Purge(appId.Value, Log);

            return $"app {appId} has been purged";
        }
    }
}