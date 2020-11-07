using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public bool EnablePublishing { get; private set; }

        public CmsManager(DataSourceFactory dataSourceFactory) : base(dataSourceFactory, "Sxc.CmsMan") { }

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
            => _runtime ?? (_runtime = ServiceProvider.Build<CmsRuntime>().Init(this, ShowDrafts, EnablePublishing, Log));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager().Init(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager().Init(this, Log));
        private BlocksManager _blocks;


    }
}
