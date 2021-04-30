using System;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;
using IApp = ToSic.Sxc.Apps.IApp;


namespace ToSic.Sxc.Oqt.Server.Controllers
{
    public abstract class OqtStatefulControllerBase : OqtControllerBase
    {
        protected readonly IServiceProvider ServiceProvider;
        private readonly OqtState _oqtState;

        protected OqtStatefulControllerBase(StatefulControllerDependencies dependencies) : base()
        {
            ServiceProvider = dependencies.ServiceProvider;
            _oqtState = dependencies.OqtState.Value.Init(GetRequest);

            dependencies.CtxResolver.AttachRealBlock(() => GetBlock());
            dependencies.CtxResolver.AttachBlockContext(GetContext);
        }

        private HttpRequest GetRequest() => Request;

        protected IContextOfBlock GetContext() => _oqtState.GetContext();

        protected IBlock GetBlock(bool allowNoContextFound = true) => _oqtState.GetBlock(allowNoContextFound);

        protected IApp GetApp(int appId) => _oqtState.GetApp(appId);
    }
}
