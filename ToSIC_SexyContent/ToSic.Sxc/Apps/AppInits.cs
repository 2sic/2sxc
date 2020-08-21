using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Apps
{
    public static class AppInits
    {
        public static IApp Init(this App app, IAppIdentity appIdentity, ILog log)
        {
            var buildconfig = ConfigurationProvider.Build(false, false, new LookUpEngine(log));
            return app.Init(appIdentity, buildconfig, false, log);
        }
    }
}
