using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        private readonly LazyInit<CmsRuntime> _cmsRuntime;

        public CmsManager(AppRuntimeDependencies dependencies, LazyInit<CmsRuntime> cmsRuntime) : base(dependencies,
            "Sxc.CmsMan")
        {
            _cmsRuntime = cmsRuntime.SetInit(r => r.InitWithState(AppState, ShowDrafts, Log));
        }

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

        public new CmsRuntime Read => _cmsRuntime.Ready;

        public ViewsManager Views => _views ?? (_views = new ViewsManager().Init(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager().Init(this, Log));
        private BlocksManager _blocks;


    }
}
