using ToSic.Eav;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DialogControllerReal(
    ISxcContextResolver ctxResolver,
    IUiContextBuilder uiContextBuilder,
    Generator<MultiPermissionsApp> appPermissions)
    : ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl", connect: [ctxResolver, uiContextBuilder, appPermissions]),
        IDialogController
{
    public const string LogSuffix = "Dialog";

    ///<inheritdoc />
    public DialogContextStandaloneDto Settings(int appId)
    {            
        // reset app-id if we get a info-token like -100
        if (appId < 0) appId = Eav.Constants.AppIdEmpty;

        var appContext = appId != Eav.Constants.AppIdEmpty ? ctxResolver.GetBlockOrSetApp(appId) : null;

        // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
        if (appContext != null)
        {
            var appAndPerms = appPermissions.New().Init(appContext, appContext.AppReader);
            if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                throw HttpException.PermissionDenied(error);
        }

        var cb = uiContextBuilder.InitApp(appContext?.AppReader);

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