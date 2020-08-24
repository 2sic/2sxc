using DotNetNuke.Entities.Users;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class SxcAppForWebApi
    {
        /// <summary>
        /// Get an app - but only allow zone change if super-user
        /// </summary>
        /// <returns></returns>
        internal static IApp AppBasedOnUserPermissions(int zoneId, int appId, UserInfo user, ILog log)
        {
            var wrapLog = log.Call<IApp>($"superuser: {user.IsSuperUser}");
            var app = user.IsSuperUser
                ? Factory.Resolve<Apps.App>().Init(new AppIdentity(zoneId, appId), 
                    ConfigurationProvider.Build(true, true, new LookUpEngine(log)), true, log)
                : Dnn.Factory.App(appId, false, parentLog: log);
            return wrapLog(null, app);
        }

    }
}