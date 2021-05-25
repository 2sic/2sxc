using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Apps
{
    public static class AppInits
    {
        //private static IApp InitWithoutBlock(this App app, AppConfigDelegate confProvider, IAppIdentity appIdentity, ILog log, bool showDrafts)
        //{
        //    var buildConfig = confProvider.Build(showDrafts);
        //    return app.Init(appIdentity, buildConfig, log);
        //}

        //public static IApp Init(this App app, IServiceProvider sp, int appId, ILog log, bool showDrafts = false)
        //{
        //    var appIdentity = new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId);
        //    var confProvider = sp.Build<AppConfigDelegate>().Init(log);
        //    return app.InitWithoutBlock(confProvider, appIdentity, log, showDrafts);
        //}

        public static IApp Init(this App app, IServiceProvider sp, int appId, ILog log, IBlock optionalBlock = null, bool showDrafts = false)
        {
            var appIdentity = new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId);
            var confProvider = sp.Build<AppConfigDelegate>().Init(log);
            var buildConfig = (optionalBlock == null)
                ? confProvider.Build(showDrafts) // return app.InitWithoutBlock(confProvider, appIdentity, log, showDrafts);
                /*var buildConfig =*/
                : confProvider.Build(optionalBlock);
            return app.Init(appIdentity, buildConfig, log);
        }
    }
}
