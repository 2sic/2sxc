using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        internal bool EnablePublishing { get; }

        public CmsRuntime(IAppIdentity app, ILog parentLog, bool showDrafts, bool enablePublishing) : base("Sxc.CmsRt")
        {
            Init(app, showDrafts, parentLog);
            EnablePublishing = enablePublishing;
        }

        public ViewsRuntime Views => _views ?? (_views = new ViewsRuntime().Init(this, Log));
        private ViewsRuntime _views;

        public BlocksRuntime Blocks => _blocks ?? (_blocks = new BlocksRuntime().Init(this, Log));
        private BlocksRuntime _blocks;

    }
}
