using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        public CmsRuntime(AppRuntimeDependencies dependencies) : base(dependencies, "Sxc.CmsRt") { }

        public new CmsRuntime Init(IAppIdentity app, bool showDrafts, ILog parentLog) 
            => base.Init(app, showDrafts, parentLog) as CmsRuntime;

        public new CmsRuntime InitWithState(AppState appState, bool showDrafts, ILog parentLog) 
            => base.InitWithState(appState, showDrafts, parentLog) as CmsRuntime;

        public ViewsRuntime Views => _views ?? (_views = ServiceProvider.Build<ViewsRuntime>().Init(this, Log));
        private ViewsRuntime _views;

        public BlocksRuntime Blocks => _blocks ?? (_blocks = ServiceProvider.Build<BlocksRuntime>().Init(this, Log));
        private BlocksRuntime _blocks;
    }
}
