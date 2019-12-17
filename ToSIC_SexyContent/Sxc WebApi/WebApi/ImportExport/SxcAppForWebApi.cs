using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.SxcTemp;
using Factory = ToSic.SexyContent.Environment.Dnn7.Factory;

namespace ToSic.Sxc.WebApi.ImportExport
{
    public class SxcAppForWebApi
    {
        //private IApp App { get; }

        //internal static IApp AppBasedOnUserPermissions(int zoneId, int appId, UserInfo user, ILog log)
        //    => GetBasedOnUserPermissions(zoneId, appId, user, log).App;


        internal static IApp AppBasedOnUserPermissions(int zoneId, int appId, UserInfo user, ILog log)
        {
            var wrapLog = log.Call<IApp>($"superuser: {user.IsSuperUser}");
            var app = user.IsSuperUser
                ? new Apps.App(new DnnTenant(PortalSettings.Current), zoneId, appId, 
                    ConfigurationProvider.Build(true, true, new LookUpEngine(log)), true, log)
                // GetApp(zoneId, appId, log) // only super-user may switch to another zone for export
                : Dnn.Factory.App(appId, false, parentLog: log);// GetApp(appId, false, log);
            return wrapLog(null, app);
        }

        //private static IApp GetApp(int appId, bool versioningEnabled, ILog log)
        //{

        //    return Dnn.Factory.App(appId, versioningEnabled, parentLog: log);
        //}

        //private static IApp GetApp(int zoneId, int appId, ILog log) 
        //    => new Apps.App(
        //        new DnnTenant(PortalSettings.Current),
        //        zoneId, appId,
        //        ConfigurationProvider.Build(true, true,
        //            new LookUpEngine(log)),
        //        true, log);
    }
}