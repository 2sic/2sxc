using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager
    {
        private readonly LazySvc<BlocksManager> _blocksManager;

        public CmsManager(
            MyServices services,
            LazySvc<BlocksManager> blocksManager
            ) : base(services, "Sxc.CmsMan")
        {
            ConnectServices(
                //_cmsRuntime = cmsRuntime.SetInit(r => r.InitWithState(AppState, ShowDrafts)),
                // _viewsManager = viewsManager.SetInit(v => v.ConnectTo(this)),
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

        public BlocksManager Blocks => _blocks.Get(() => _blocksManager.Value);
        private readonly GetOnce<BlocksManager> _blocks = new GetOnce<BlocksManager>();


    }
}
