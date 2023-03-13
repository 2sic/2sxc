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

        public override void OnActionExecuting(ActionExecutingContext context) => Log.Do(() =>
        {
            base.OnActionExecuting(context);

            var getBlock = GetService<OqtGetBlock>();
            CtxResolver = getBlock.TryToLoadBlockAndAttachToResolver();
            BlockOptional = CtxResolver.BlockOrNull();
        });

        protected IBlock BlockOptional { get; private set; }

        protected IApp GetApp(int appId)
            => GetService<Apps.App>().Init(appId, BlockOptional);
    }
}
