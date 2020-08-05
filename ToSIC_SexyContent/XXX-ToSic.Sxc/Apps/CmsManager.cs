using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public bool EnablePublishing { get; }

        public CmsManager(IAppIdentityWithPublishingState app, ILog parentLog) 
            : base(app, parentLog) 
            => EnablePublishing = app.EnablePublishing;

        public CmsManager(IAppIdentity app, bool showDrafts, bool enablePublishing, ILog parentLog) 
            : base(app, parentLog) 
            => EnablePublishing = enablePublishing;

        public new CmsRuntime Read 
            => _runtime ?? (_runtime = new CmsRuntime(this, Log, ShowDrafts, EnablePublishing));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager(this, Log));
        private BlocksManager _blocks;


    }
}
