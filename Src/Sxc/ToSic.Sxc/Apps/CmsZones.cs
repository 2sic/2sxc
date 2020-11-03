using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsZones: ZoneRuntime
    {
        public CmsZones() : base("Sxc.ZoneRt") { }

        public new CmsZones Init(int zoneId, ILog parentLog)
        {
            base.Init(zoneId, parentLog);
            return this;
        }

        public AppsRuntime AppsRt => _apps ?? (_apps = new AppsRuntime(this, Log));
        private AppsRuntime _apps;

        public AppsManager AppsMan => _appsMan ?? (_appsMan = new AppsManager(this, Log));
        private AppsManager _appsMan;
    }
}
