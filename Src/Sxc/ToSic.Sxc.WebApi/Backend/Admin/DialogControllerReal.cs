using ToSic.Eav;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DialogControllerReal: ServiceBase, IDialogController
{
    private readonly Generator<MultiPermissionsApp> _appPermissions;
    public const string LogSuffix = "Dialog";

    public DialogControllerReal(
        ISxcContextResolver ctxResolver,
        IUiContextBuilder uiContextBuilder,
        Generator<MultiPermissionsApp> appPermissions) : base($"{EavLogs.WebApi}.{LogSuffix}Rl")
    {
        ConnectServices(
            _ctxResolver = ctxResolver,
            _uiContextBuilder = uiContextBuilder,
            _appPermissions = appPermissions
        );
    }
    private readonly ISxcContextResolver _ctxResolver;
    private readonly IUiContextBuilder _uiContextBuilder;

    ///<inheritdoc />
    public DialogContextStandaloneDto Settings(int appId)
    {            
        // reset app-id if we get a info-token like -100
        if (appId < 0) appId = Eav.Constants.AppIdEmpty;

        var appContext = appId != Eav.Constants.AppIdEmpty ? _ctxResolver.GetBlockOrSetApp(appId) : null;

        // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
        if (appContext != null)
        {
            var appAndPerms = _appPermissions.New().Init(appContext, appContext.AppState);
            if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                throw HttpException.PermissionDenied(error);
        }

        var cb = _uiContextBuilder.InitApp(appContext?.AppState);

        return new()
        {
            Context = cb.Get(Ctx.General, CtxEnable.All),
        };
    }

}

public class DialogContextStandaloneDto
{
    public ContextDto Context { get; set; }
}