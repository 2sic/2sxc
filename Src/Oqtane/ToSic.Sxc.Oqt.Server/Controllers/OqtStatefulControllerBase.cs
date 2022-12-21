using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase<TRealController> : OqtControllerBase<TRealController> where TRealController : class, IHasLog
    {
        protected OqtStatefulControllerBase(string logSuffix): base(logSuffix) { }

        protected IContextResolver CtxResolver;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var wrapLog = Log.Fn();

            base.OnActionExecuting(context);

            var getBlock = GetService<OqtGetBlock>().Init(Log);
            CtxResolver = getBlock.TryToLoadBlockAndAttachToResolver();
            BlockOptional = CtxResolver.RealBlockOrNull();
            wrapLog.Done();
        }

        // TODO: 2021-09-20 2dm this should probably be removed - I don't think the context should be available on this class, but I'm not sure 
        protected IContextOfBlock GetContext() => BlockOptional?.Context;

        protected IBlock BlockOptional { get; private set; }

        protected IApp GetApp(int appId)
            => GetService<Apps.App>().Init(Log).Init(appId, BlockOptional);
    }
}
