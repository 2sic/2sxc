using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase : OqtControllerBase
    {
        protected OqtState OqtState;
        protected IContextResolver CtxResolver;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var wrapLog = Log.Call();

            base.OnActionExecuting(context);
            
            OqtState = ServiceProvider.Build<OqtState>().Init(Log) ;// dependencies.OqtState.Init(Log);
            CtxResolver = ServiceProvider.Build<IContextResolver>() ;// dependencies.CtxResolver;
            CtxResolver.AttachRealBlock(() => GetBlock());
            CtxResolver.AttachBlockContext(GetContext);
            wrapLog(null);
        }

        protected IContextOfBlock GetContext() => OqtState.GetContext();

        protected IBlock GetBlock(bool allowNoContextFound = true) => OqtState.GetBlock(allowNoContextFound);

        protected IApp GetApp(int appId) => OqtState.GetApp(appId);
    }
}
