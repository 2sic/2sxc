using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
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
            _blocksRuntime = blocksRuntime.SetInit(r => r.Init(this, Log));
            _viewsRuntime = viewsRuntime.SetInit(r => r.Init(this, Log));
        }

        public new CmsRuntime Init(IAppIdentity app, bool showDrafts, ILog parentLog) 
            => base.Init(app, showDrafts, parentLog) as CmsRuntime;

        public new CmsRuntime InitWithState(AppState appState, bool showDrafts, ILog parentLog) 
            => base.InitWithState(appState, showDrafts, parentLog) as CmsRuntime;

        public ViewsRuntime Views => _viewsRuntime.Value;

        public BlocksRuntime Blocks => _blocksRuntime.Value;
    }
}
