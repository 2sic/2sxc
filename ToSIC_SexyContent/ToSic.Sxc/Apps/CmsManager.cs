using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager
    {
        public CmsManager(IAppIdentity app, ILog parentLog) : base(app, parentLog)
        {
        }

        public CmsManager(int zoneId, int appId, ILog parentLog = null) : base(zoneId, appId, parentLog)
        {

        }

        public CmsManager(int appId, ILog parentLog) : base(appId, parentLog)
        {

        }


        public ViewsManager Views => _views ?? (_views = new ViewsManager(this, Log));
        private ViewsManager _views;

        public ViewsRuntime ViewReadTemp => _viewReadTemp ?? (_viewReadTemp = new ViewsRuntime(ZoneId, AppId, Log));
        private ViewsRuntime _viewReadTemp;


    }
}
