using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DialogControllerReal(
    ISxcCurrentContextService ctxService,
    IUiContextBuilder uiContextBuilder,
    Generator<MultiPermissionsApp> appPermissions)
    : ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl", connect: [ctxService, uiContextBuilder, appPermissions]),
        IDialogController
{
    public const string LogSuffix = "Dialog";

    ///<inheritdoc />
    public DialogContextStandaloneDto Settings(int appId)
    {            
        // reset app-id if we get a info-token like -100
        if (appId < 0) appId = KnownAppsConstants.AppIdEmpty;

        var appContext = appId != KnownAppsConstants.AppIdEmpty ? ctxService.GetExistingAppOrSet(appId) : null;

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