namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        public string Purge(int? appId = null)
        {
            ThrowIfNotSuperUser();

            if (appId == null)
                return "please add appid to the url parameters";

            SystemManager.Init(Log).PurgeApp(appId.Value);

            return $"app {appId} has been purged";
        }
    }
}
