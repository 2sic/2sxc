using ToSic.Eav.Apps;
using ToSic.Eav.DataSources;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Apps
{
    public class CmsRuntime: AppRuntime
    {
        public CmsRuntime(DataSourceFactory dataSourceFactory) : base(dataSourceFactory, "Sxc.CmsRt") { }

        public new CmsRuntime Init(IAppIdentity app, bool showDrafts, ILog parentLog) 
            => base.Init(app, showDrafts, parentLog) as CmsRuntime;

        public ViewsRuntime Views => _views ?? (_views = ServiceProvider.Build<ViewsRuntime>().Init(this, Log));
        private ViewsRuntime _views;

        public BlocksRuntime Blocks => _blocks ?? (_blocks = new BlocksRuntime().Init(this, Log));
        private BlocksRuntime _blocks;
    }
}
