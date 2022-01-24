using System;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public class AdminBackend: WebApiBackendBase<AdminBackend>
    {
        private readonly IContextResolver _ctxResolver;
        private readonly IUiContextBuilder _uiContextBuilder;

        public AdminBackend(IServiceProvider serviceProvider, IContextResolver ctxResolver, IUiContextBuilder uiContextBuilder) : base(serviceProvider, "Bck.Admin")
        {
            _ctxResolver = ctxResolver;
            _uiContextBuilder = uiContextBuilder;
        }

        public DialogContextStandalone DialogSettings(int appId)
        {            
            // reset app-id if we get a info-token like -100
            if (appId < 0) appId = Eav.Constants.AppIdEmpty;

            var appContext = appId != Eav.Constants.AppIdEmpty ? _ctxResolver.BlockOrApp(appId) : null;

            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appContext != null)
            {
                var appAndPerms = ServiceProvider.Build<MultiPermissionsApp>().Init(appContext, appContext.AppState, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
            }

            var cb = _uiContextBuilder.InitApp(appContext?.AppState, Log);

            return new DialogContextStandalone
            {
                Context = cb.Get(Ctx.All, CtxEnable.All),
            };
        }

    }

    public class DialogContextStandalone
    {
        public ContextDto Context { get; set; }
    }

}
