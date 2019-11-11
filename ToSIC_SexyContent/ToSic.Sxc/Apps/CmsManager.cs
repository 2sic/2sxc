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

        public new CmsRuntime Read => _runtime ?? (_runtime = new CmsRuntime(Data, Log));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager(this, Log));
        private ViewsManager _views;


    }
}
