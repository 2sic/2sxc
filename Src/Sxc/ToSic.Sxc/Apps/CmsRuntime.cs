using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        private readonly LazyInit<BlocksRuntime> _blocksRuntime;
        private readonly LazyInit<ViewsRuntime> _viewsRuntime;

        public CmsRuntime(AppRuntimeDependencies dependencies, 
            LazyInit<EntityRuntime> entityRuntime,
            LazyInit<MetadataRuntime> metadataRuntime,
            LazyInit<ContentTypeRuntime> contentTypeRuntime,
            LazyInit<QueryRuntime> queryRuntime, 
            LazyInit<ViewsRuntime> viewsRuntime, 
            LazyInit<BlocksRuntime> blocksRuntime) 
            : base(dependencies, entityRuntime, metadataRuntime, contentTypeRuntime, queryRuntime, "Sxc.CmsRt")
        {
            _blocksRuntime = blocksRuntime.SetInit(r => r.Init(Log).ConnectTo(this));
            _viewsRuntime = viewsRuntime.SetInit(r => r.Init(Log).ConnectTo(this));
        }

        public new CmsRuntime InitWithState(AppState appState, bool showDrafts)
        {
            return base.InitWithState(appState, showDrafts) as CmsRuntime;
        }

        public ViewsRuntime Views => _viewsRuntime.Value;

        public BlocksRuntime Blocks => _blocksRuntime.Value;
    }
}
