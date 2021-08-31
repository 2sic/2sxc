using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsZones: ZoneRuntime
    {
        #region Constructor / DI

        private readonly Lazy<AppsRuntime> _appsRuntimeLazy;
        private readonly Lazy<AppsManager> _appsManagerLazy;
        public CmsZones(Lazy<AppsRuntime> appsRuntimeLazy, Lazy<AppsManager> appsManagerLazy, IAppStates appStates) : base("Sxc.ZoneRt")
        {
            _appsRuntimeLazy = appsRuntimeLazy;
            _appsManagerLazy = appsManagerLazy;
        }

        public new CmsZones Init(int zoneId, ILog parentLog)
        {
            base.Init(zoneId, parentLog);
            return this;
        }

        #endregion

        public AppsRuntime AppsRt => _apps ?? (_apps = _appsRuntimeLazy.Value.Init(this, Log));
        private AppsRuntime _apps;

        public AppsManager AppsMan => _appsMan ?? (_appsMan = _appsManagerLazy.Value.Init(this, Log));
        private AppsManager _appsMan;
    }
}
