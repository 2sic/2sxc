using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public CmsManager(AppRuntimeDependencies dependencies) : base(dependencies, "Sxc.CmsMan") { }

        public CmsManager Init(IAppIdentityWithPublishingState app, ILog parentLog)
        {
            base.Init(app, parentLog);
            return this;
        }

        public new CmsManager Init(IAppIdentity app, bool showDrafts, ILog parentLog)
        {
            base.Init(app, showDrafts, parentLog);
            return this;
        }
        public CmsManager Init(IContextOfApp context, ILog parentLog)
        {
            base.Init(context.AppState, context.UserMayEdit, parentLog);
            return this;
        }

        public new CmsManager InitWithState(AppState app, bool showDrafts, ILog parentLog)
        {
            base.InitWithState(app, showDrafts, parentLog);
            return this;
        }

        public new CmsRuntime Read 
            => _runtime ?? (_runtime = ServiceProvider.Build<CmsRuntime>().InitWithState(AppState, ShowDrafts, Log));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager().Init(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager().Init(this, Log));
        private BlocksManager _blocks;


    }
}
