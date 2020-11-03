using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        internal bool EnablePublishing { get; set; }

        public CmsRuntime(IServiceProvider serviceProvider): base(serviceProvider, "Sxc.CmsRt") { }

        public CmsRuntime Init(IAppIdentity app, bool showDrafts, bool enablePublishing, ILog parentLog)
        {
            Init(app, showDrafts, parentLog);
            EnablePublishing = enablePublishing;
            return this;
        }

        public ViewsRuntime Views => _views ?? (_views = new ViewsRuntime().Init(this, Log));
        private ViewsRuntime _views;

        public BlocksRuntime Blocks => _blocks ?? (_blocks = new BlocksRuntime().Init(this, Log));
        private BlocksRuntime _blocks;

    }
}
