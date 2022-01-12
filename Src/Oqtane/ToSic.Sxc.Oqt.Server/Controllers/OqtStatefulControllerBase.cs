using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase : OqtControllerBase
    {
        //protected OqtState OqtState;
        protected IContextResolver CtxResolver;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var wrapLog = Log.Call();

            base.OnActionExecuting(context);

            var getBlock = ServiceProvider.Build<OqtGetBlock>().Init(Log);
            CtxResolver = getBlock.TryToLoadBlockAndAttachToResolver();
            BlockOptional = CtxResolver.RealBlockOrNull();
            //var blockOptional = BlockOptional = getBlock.GetBlock();
            //OqtState = ServiceProvider.Build<OqtState>().Init(Log) ;// dependencies.OqtState.Init(Log);
            //CtxResolver = ServiceProvider.Build<IContextResolver>() ;// dependencies.CtxResolver;
            //CtxResolver.AttachRealBlock(() => blockOptional);
            //CtxResolver.AttachBlockContext(() => blockOptional?.Context);
            wrapLog(null);
        }


        // TODO: 2021-09-20 2dm this should probably be removed - I don't think the context should be available on this class, but I'm not sure 
        protected IContextOfBlock GetContext() => BlockOptional?.Context; // OqtState.GetContext();

        //protected IBlock GetBlock(bool allowNoContextFound = true) => OqtState.GetBlock(allowNoContextFound);

        protected IBlock BlockOptional { get; private set; }

        protected IApp GetApp(int appId)
            // => OqtState.GetApp(appId);
            => ServiceProvider.Build<App>().Init(ServiceProvider, appId, Log, BlockOptional);
    }
}
