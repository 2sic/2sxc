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
        public static IApp Init(this App app, IServiceProvider sp, int appId, ILog log, IBlock optionalBlock = null, bool showDrafts = false)
        {
            var appStates = sp.Build<IAppStates>();
            var appIdentity = appStates.IdentityOfApp(appId);
            var confProvider = sp.Build<AppConfigDelegate>().Init(log);
            var buildConfig = (optionalBlock == null)
                ? confProvider.Build(showDrafts)
                : confProvider.Build(optionalBlock);
            return app.Init(appIdentity, buildConfig, log);
        }
    }
}
