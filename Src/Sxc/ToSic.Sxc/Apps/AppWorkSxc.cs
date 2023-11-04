using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Work;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.CmsSys;
using ToSic.Sxc.Apps.Work;

namespace ToSic.Sxc.Apps
{
    public class AppWorkSxc: ServiceBase
    {
        private readonly Generator<WorkViews> _appViewGen;
        public AppWork AppWork { get; }

        public AppWorkSxc(AppWork appWork, Generator<WorkViews> appViewGen) : base("ASP.Main")
        {
            _appViewGen = appViewGen;
            AppWork = appWork;
        }

        
        public WorkViews AppViews(IAppWorkCtxPlus ctx = default, AppState state = default, IAppIdentity identity = default, int? appId = default)
            => _appViewGen.New().InitContext(AppWork.CtxSvc.CtxPlus(ctx: ctx, identity: identity, state: state, appId: appId));
    }
}
