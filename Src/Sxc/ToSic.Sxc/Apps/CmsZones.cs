using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Apps
{
    public class CmsZones: ZoneRuntime
    {
        #region Constructor / DI

        private readonly LazySvc<AppsRuntime> _appsRuntimeLazy;
        private readonly LazySvc<AppsManager> _appsManagerLazy;
        public CmsZones(LazySvc<AppsRuntime> appsRuntimeLazy, LazySvc<AppsManager> appsManagerLazy) : base("Sxc.ZoneRt") =>
            ConnectServices(
                _appsRuntimeLazy = appsRuntimeLazy.SetInit(x => x.ConnectTo(this)),
                _appsManagerLazy = appsManagerLazy.SetInit(x => x.ConnectTo(this))
            );

        #endregion

        public AppsRuntime AppsRt => _appsRuntimeLazy.Value;

        public AppsManager AppsMan => _appsManagerLazy.Value;
    }
}
