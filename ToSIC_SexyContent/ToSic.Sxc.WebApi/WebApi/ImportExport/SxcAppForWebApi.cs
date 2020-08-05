using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;

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
                ? new Apps.App(new DnnTenant(PortalSettings.Current), zoneId, appId, 
                    ConfigurationProvider.Build(true, true, new LookUpEngine(log)), true, log)
                : Dnn.Factory.App(appId, false, parentLog: log);
            return wrapLog(null, app);
        }

    }
}