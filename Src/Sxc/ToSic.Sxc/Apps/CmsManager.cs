using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager //, IAppIdentityWithPublishingState
    {
        private readonly LazySvc<ViewsManager> _viewsManager;
        private readonly LazySvc<BlocksManager> _blocksManager;
        private readonly LazySvc<CmsRuntime> _cmsRuntime;

        public CmsManager(
            MyServices services, LazySvc<CmsRuntime> cmsRuntime,
            LazySvc<ViewsManager> viewsManager,
            LazySvc<BlocksManager> blocksManager
            ) : base(services, "Sxc.CmsMan")
        {
            ConnectServices(
                _cmsRuntime = cmsRuntime.SetInit(r => r.InitWithState(AppState, ShowDrafts)),
                _viewsManager = viewsManager.SetInit(v => v.ConnectTo(this)),
                _blocksManager = blocksManager.SetInit(b => b.ConnectTo(this))
            );
        }

        public CmsManager Init(IApp app)
        {
            base.Init(app);
            return this;
        }

        public CmsManager Init(IContextOfApp context)
        {
            this.InitQ(context.AppState);
            return this;
        }

        public new CmsManager InitWithState(AppState app, bool? showDrafts = null)
        {
            base.InitWithState(app);
            return this;
        }

        public new CmsRuntime Read => _cmsRuntime.Value;

        public ViewsManager Views => _views.Get(() => _viewsManager.Value);
        private readonly GetOnce<ViewsManager> _views = new GetOnce<ViewsManager>();

        public BlocksManager Blocks => _blocks.Get(() => _blocksManager.Value);
        private readonly GetOnce<BlocksManager> _blocks = new GetOnce<BlocksManager>();


    }
}
