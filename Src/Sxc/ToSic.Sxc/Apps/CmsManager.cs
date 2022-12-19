using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Repository.Efc;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        private readonly LazyInit<CmsRuntime> _cmsRuntime;

        public CmsManager(AppRuntimeDependencies dependencies, 
            LazyInit<AppRuntime> appRuntime,
            LazyInit<DbDataController> dbDataController,
            LazyInit<EntitiesManager> entitiesManager,
            LazyInit<QueryManager> queryManager,
            LazyInit<CmsRuntime> cmsRuntime
            ) : base(dependencies, appRuntime, dbDataController, entitiesManager, queryManager, "Sxc.CmsMan")
        {
            _cmsRuntime = cmsRuntime.SetInit(r => r.Init(Log).InitWithState(AppState, ShowDrafts));
        }

        public CmsManager Init(IAppIdentityWithPublishingState app)
        {
            base.Init(app);
            return this;
        }

        //public new CmsManager Init(IAppIdentity app, bool showDrafts)
        //{
        //    this.InitQ(app, showDrafts);
        //    return this;
        //}
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

        public ViewsManager Views => _views ?? (_views = new ViewsManager().Init(Log).ConnectTo(this));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager().Init(Log).ConnectTo(this));
        private BlocksManager _blocks;


    }
}
