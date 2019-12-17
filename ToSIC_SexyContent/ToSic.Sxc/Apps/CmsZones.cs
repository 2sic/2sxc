using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Apps
{
    public class CmsZones: ZoneRuntime
    {
        public CmsZones(int zoneId, IAppEnvironment environment, ILog parentLog) : base(zoneId, parentLog)
        {
            Env = environment;
        }

        public AppsRuntime AppsRt => _apps ?? (_apps = new AppsRuntime(this, Log));
        private AppsRuntime _apps;

        public AppsManager AppsMan => _appsMan ?? (_appsMan = new AppsManager(this, Log));
        private AppsManager _appsMan;


        public IAppEnvironment Env { get; }
    }
}
