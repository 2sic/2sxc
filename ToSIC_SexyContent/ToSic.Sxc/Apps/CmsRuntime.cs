using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        public CmsRuntime(int zoneId, int appId, ILog parentLog) : base(zoneId, appId, parentLog)
        {
        }

        public CmsRuntime(IAppIdentity app, ILog parentLog) : base(app, parentLog)
        {
        }

        public CmsRuntime(int appId, ILog parentLog) : base(appId, parentLog)
        {
        }


        public ViewsRuntime Views => _views ?? (_views = new ViewsRuntime(this, Log));
        private ViewsRuntime _views;

    }
}
