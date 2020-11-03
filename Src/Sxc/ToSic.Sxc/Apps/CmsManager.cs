using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public bool EnablePublishing { get; private set; }

        public CmsManager() : base("Sxc.CmsMan") { }

        public CmsManager Init(IAppIdentityWithPublishingState app, ILog parentLog)
        {
            base.Init(app, parentLog);
            EnablePublishing = app.EnablePublishing;
            return this;
        }

        public CmsManager Init(IAppIdentity app, bool showDrafts, bool enablePublishing, ILog parentLog)
        {
            base.Init(app, showDrafts, parentLog);
            EnablePublishing = enablePublishing;
            return this;
        }

        public new CmsRuntime Read 
            => _runtime ?? (_runtime = Eav.Factory.Resolve<CmsRuntime>().Init(this, ShowDrafts, EnablePublishing, Log));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager().Init(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager().Init(this, Log));
        private BlocksManager _blocks;


    }
}
