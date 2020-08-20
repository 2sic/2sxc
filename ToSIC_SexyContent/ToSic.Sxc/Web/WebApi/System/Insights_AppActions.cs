using ToSic.Eav.Apps;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Ins
    {
        public string Purge(int? appId = null)
        {
            ThrowIfNotSuperUser();

            if (appId == null)
                return "please add appid to the url parameters";

            SystemManager.Purge(appId.Value, Log);

            return $"app {appId} has been purged";
        }
    }
}
