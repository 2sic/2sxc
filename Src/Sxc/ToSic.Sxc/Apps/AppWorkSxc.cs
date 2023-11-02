using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppSys;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.CmsSys;

namespace ToSic.Sxc.Apps
{
    public class AppWorkSxc: ServiceBase
    {
        private readonly Generator<AppViews> _appViewGen;
        public AppWork AppWork { get; }

        public AppWorkSxc(AppWork appWork, Generator<AppViews> appViewGen) : base("ASP.Main")
        {
            _appViewGen = appViewGen;
            AppWork = appWork;
        }

        
        public AppViews AppViews(IAppWorkCtxPlus ctx = default, AppState state = default, IAppIdentity identity = default, int? appId = default)
            => _appViewGen.New().Setup(AppWork.CtxSvc.CtxPlus(ctx: ctx, identity: identity, state: state, appId: appId));
    }
}
