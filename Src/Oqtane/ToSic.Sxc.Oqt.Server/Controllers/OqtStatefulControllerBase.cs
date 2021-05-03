using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
        protected IServiceProvider ServiceProvider;
        private OqtState _oqtState;

        protected OqtStatefulControllerBase() : base() { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Log.Add($"Url: {context.HttpContext.Request.GetDisplayUrl()}");

            base.OnActionExecuting(context);

            ServiceProvider = context.HttpContext.RequestServices;
            var dependencies = ServiceProvider.Build<StatefulControllerDependencies>();

            _oqtState = dependencies.OqtState.Value;

            dependencies.CtxResolver.AttachRealBlock(() => GetBlock());
            dependencies.CtxResolver.AttachBlockContext(GetContext);
        }

        protected IContextOfBlock GetContext() => _oqtState.GetContext();

        protected IBlock GetBlock(bool allowNoContextFound = true) => _oqtState.GetBlock(allowNoContextFound);

        protected IApp GetApp(int appId) => _oqtState.GetApp(appId);
    }
}
