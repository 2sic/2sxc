using System;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public class DialogControllerReal: WebApiBackendBase<DialogControllerReal>, IDialogController
    {
        private readonly IContextResolver _ctxResolver;
        private readonly IUiContextBuilder _uiContextBuilder;

        public DialogControllerReal(IServiceProvider serviceProvider, IContextResolver ctxResolver, IUiContextBuilder uiContextBuilder) : base(serviceProvider, "Bck.Admin")
        {
            _ctxResolver = ctxResolver;
            _uiContextBuilder = uiContextBuilder;
        }

        ///<inheritdoc />
        public DialogContextStandaloneDto Settings(int appId)
        {            
            // reset app-id if we get a info-token like -100
            if (appId < 0) appId = Eav.Constants.AppIdEmpty;

            var appContext = appId != Eav.Constants.AppIdEmpty ? _ctxResolver.BlockOrApp(appId) : null;

            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appContext != null)
            {
                var appAndPerms = GetService<MultiPermissionsApp>().Init(appContext, appContext.AppState, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
            }

            var cb = _uiContextBuilder.InitApp(appContext?.AppState, Log);

            return new DialogContextStandaloneDto
            {
                Context = cb.Get(Ctx.All, CtxEnable.All),
            };
        }

    }

    public class DialogContextStandaloneDto
    {
        public ContextDto Context { get; set; }
    }

}
