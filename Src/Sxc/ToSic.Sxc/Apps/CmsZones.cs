using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps
{
    public class CmsZones: ZoneRuntime
    {
        #region Constructor / DI

        private readonly LazyInit<AppsRuntime> _appsRuntimeLazy;
        private readonly LazyInit<AppsManager> _appsManagerLazy;
        public CmsZones(LazyInit<AppsRuntime> appsRuntimeLazy, LazyInit<AppsManager> appsManagerLazy) : base("Sxc.ZoneRt") =>
            ConnectServices(
                _appsRuntimeLazy = appsRuntimeLazy,
                _appsManagerLazy = appsManagerLazy
            );

        #endregion

        public AppsRuntime AppsRt => _apps ?? (_apps = _appsRuntimeLazy.Value.ConnectTo(this));
        private AppsRuntime _apps;

        public AppsManager AppsMan => _appsMan ?? (_appsMan = _appsManagerLazy.Value.ConnectTo(this));
        private AppsManager _appsMan;
    }
}
