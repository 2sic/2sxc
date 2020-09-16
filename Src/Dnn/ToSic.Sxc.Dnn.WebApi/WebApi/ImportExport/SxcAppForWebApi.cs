using DotNetNuke.Entities.Users;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class ImpExpHelpers
    {
        /// <summary>
        /// Get an app - but only allow zone change if super-user
        /// </summary>
        /// <returns></returns>
        internal static IApp GetAppAndCheckZoneSwitchPermissions(int zoneId, int appId, UserInfo user, int contextZoneId, ILog log)
        {
            var wrapLog = log.Call<IApp>($"superuser: {user.IsSuperUser}");
            if (!user.IsSuperUser && zoneId != contextZoneId)
            {
                wrapLog("error", null);
                throw Eav.WebApi.Errors.HttpException.PermissionDenied("Tried to access app from another zone. Requires SuperUser permissions.");
            }

            var app = Factory.Resolve<Apps.App>().Init(new AppIdentity(zoneId, appId),
                ConfigurationProvider.Build(true, true, new LookUpEngine(log)), true, log);
            return wrapLog(null, app);
        }

    }
}