using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Apps
{
    public static class AppInits
    {
        public static IApp Init(this App app, IAppIdentity appIdentity, ILog log)
        {
            var buildConfig = ConfigurationProvider.Build(false, false, new LookUpEngine(log));
            return app.Init(appIdentity, buildConfig, false, log);
        }

        public static IApp Init(this App app, int appId, ILog log, IBlockBuilder optionalBuilder = null)
        {
            var appIdentity = new AppIdentity(SystemRuntime.ZoneIdOfApp(appId), appId);
            if (optionalBuilder == null) return app.Init(appIdentity, log);
            var buildConfig = ConfigurationProvider.Build(optionalBuilder, true);
            return app.Init(appIdentity, buildConfig, false, log);
        }
    }
}
