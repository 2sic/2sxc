using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Eav.Repository.Efc;
using ToSic.Lib.Helper;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        private readonly LazySvc<ViewsManager> _viewsManager;
        private readonly LazySvc<BlocksManager> _blocksManager;
        private readonly LazySvc<CmsRuntime> _cmsRuntime;

        public CmsManager(AppRuntimeDependencies dependencies, 
            LazySvc<AppRuntime> appRuntime,
            LazySvc<DbDataController> dbDataController,
            LazySvc<EntitiesManager> entitiesManager,
            LazySvc<QueryManager> queryManager,
            LazySvc<CmsRuntime> cmsRuntime,
            LazySvc<ViewsManager> viewsManager,
            LazySvc<BlocksManager> blocksManager
            ) : base(dependencies, appRuntime, dbDataController, entitiesManager, queryManager, "Sxc.CmsMan")
        {
            ConnectServices(
                _cmsRuntime = cmsRuntime.SetInit(r => r.InitWithState(AppState, ShowDrafts)),
                _viewsManager = viewsManager.SetInit(v => v.ConnectTo(this)),
                _blocksManager = blocksManager.SetInit(b => b.ConnectTo(this))
            );
        }

        public CmsManager Init(IAppIdentityWithPublishingState app)
        {
            base.Init(app);
            return this;
        }

        public CmsManager Init(IContextOfApp context)
        {
            this.InitQ(context.AppState, context.UserMayEdit);
            return this;
        }

        public new CmsManager InitWithState(AppState app, bool showDrafts)
        {
            base.InitWithState(app, showDrafts);
            return this;
        }

        public new CmsRuntime Read => _cmsRuntime.Value;

        public ViewsManager Views => _views.Get(() => _viewsManager.Value);// new ViewsManager().Init(Log).ConnectTo(this));
        private readonly GetOnce<ViewsManager> _views = new GetOnce<ViewsManager>();

        public BlocksManager Blocks => _blocks.Get(() => _blocksManager.Value);// new BlocksManager().Init(Log).ConnectTo(this));
        private readonly GetOnce<BlocksManager> _blocks = new GetOnce<BlocksManager>();


    }
}
