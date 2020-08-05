using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        internal bool EnablePublishing { get; }

        public CmsRuntime(IAppIdentity app, ILog parentLog, bool showDrafts, bool enablePublishing) : base(app, showDrafts, parentLog)
        {
            EnablePublishing = enablePublishing;
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
