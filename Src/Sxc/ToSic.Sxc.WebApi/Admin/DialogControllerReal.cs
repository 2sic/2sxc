using System;
using ToSic.Eav;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public class DialogControllerReal: ServiceBase, IDialogController
    {
        private readonly GeneratorLog<MultiPermissionsApp> _appPermissions;
        public const string LogSuffix = "Dialog";

        public DialogControllerReal(
            IContextResolver ctxResolver,
            IUiContextBuilder uiContextBuilder,
            GeneratorLog<MultiPermissionsApp> appPermissions) : base($"{LogNames.WebApi}.{LogSuffix}Rl")
        {
            ConnectServices(
                _ctxResolver = ctxResolver,
                _uiContextBuilder = uiContextBuilder,
                _appPermissions = appPermissions
            );
        }
        private readonly IContextResolver _ctxResolver;
        private readonly IUiContextBuilder _uiContextBuilder;

        ///<inheritdoc />
        public DialogContextStandaloneDto Settings(int appId)
        {            
            // reset app-id if we get a info-token like -100
            if (appId < 0) appId = Eav.Constants.AppIdEmpty;

            var appContext = appId != Eav.Constants.AppIdEmpty ? _ctxResolver.BlockOrApp(appId) : null;

            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appContext != null)
            {
                var appAndPerms = _appPermissions.New().Init(appContext, appContext.AppState, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
            }

            var cb = _uiContextBuilder.InitApp(appContext?.AppState);

            return new DialogContextStandaloneDto
            {
                Context = cb.Get(Ctx.General, CtxEnable.All),
            };
        }

    }

    public class DialogContextStandaloneDto
    {
        public ContextDto Context { get; set; }
    }

}
