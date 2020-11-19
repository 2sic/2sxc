using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Apps
{
    public static class AppInits
    {
        public static IApp Init(this App app, AppConfigDelegate confProvider, IAppIdentity appIdentity, ILog log, bool showDrafts = false)
        {
            var buildConfig = confProvider.Build(showDrafts/*, new LookUpEngine(log)*/);
            return app.Init(appIdentity, buildConfig, log);
        }

        public static IApp Init(this App app, IServiceProvider sp, int appId, ILog log, IBlock optionalBlock = null, bool showDrafts = false)
        {
            var appIdentity = new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId);
            var confProvider = sp.Build<AppConfigDelegate>().Init(log);
            if (optionalBlock == null) return app.Init(confProvider, appIdentity, log, showDrafts);
            var buildConfig = confProvider.Build(optionalBlock, true);
            return app.Init(appIdentity, buildConfig, log);
        }
    }
}
