using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        private readonly LazySvc<BlocksRuntime> _blocksRuntime;
        private readonly LazySvc<ViewsRuntime> _viewsRuntime;

        public CmsRuntime(AppRuntimeServices services, 
            LazySvc<EntityRuntime> entityRuntime,
            LazySvc<MetadataRuntime> metadataRuntime,
            LazySvc<ContentTypeRuntime> contentTypeRuntime,
            LazySvc<QueryRuntime> queryRuntime, 
            LazySvc<ViewsRuntime> viewsRuntime, 
            LazySvc<BlocksRuntime> blocksRuntime) 
            : base(services, entityRuntime, metadataRuntime, contentTypeRuntime, queryRuntime, "Sxc.CmsRt")
        {
            ConnectServices(
                _blocksRuntime = blocksRuntime.SetInit(r => r.ConnectTo(this)),
                _viewsRuntime = viewsRuntime.SetInit(r => r.ConnectTo(this))
            );
        }

        public new CmsRuntime InitWithState(AppState appState, bool showDrafts)
        {
            return base.InitWithState(appState, showDrafts) as CmsRuntime;
        }

        public ViewsRuntime Views => _viewsRuntime.Value;

        public BlocksRuntime Blocks => _blocksRuntime.Value;
    }
}
