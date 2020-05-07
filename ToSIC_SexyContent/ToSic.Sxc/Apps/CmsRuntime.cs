using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        public bool WithPublishing { get; }

        //public CmsRuntime(int zoneId, int appId, ILog parentLog, bool showDraft, bool withPublishing) 
        //    : base(zoneId, appId, parentLog)
        //{
        //    ShowDrafts = showDraft;
        //    WithPublishing = withPublishing;
        //}

        public CmsRuntime(IAppIdentity app, ILog parentLog, bool showDrafts, bool withPublishing) : base(app, showDrafts, parentLog)
        {
            WithPublishing = withPublishing;
        }

        public CmsRuntime(int appId, ILog parentLog, bool showDrafts) : base(appId, showDrafts, parentLog)
        {
        }


        public ViewsRuntime Views => _views ?? (_views = new ViewsRuntime(this, Log));
        private ViewsRuntime _views;

        public BlocksRuntime Blocks => _blocks ?? (_blocks = new BlocksRuntime(this, Log));
        private BlocksRuntime _blocks;

    }
}
