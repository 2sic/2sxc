using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Sys;

namespace ToSic.Sxc.Backend.Admin;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DialogControllerReal(
    ZoneSwitchPermissionCheck zoneSwitchPermissionCheck,
    ISxcCurrentContextService ctxService,
    IUiContextBuilder uiContextBuilder)
    : ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl", connect: [zoneSwitchPermissionCheck, ctxService, uiContextBuilder]),
        IDialogController
{
    public const string LogSuffix = "Dialog";

    ///<inheritdoc />
    public DialogContextStandaloneDto Settings(int appId)
    {            
        // reset app-id if we get a info-token like -100
        if (appId < 0)
            appId = KnownAppsConstants.AppIdEmpty;

        var appContext = appId != KnownAppsConstants.AppIdEmpty
            ? ctxService.ShareAppStateOptimized(appId)
            : null;
        var appReader = appContext?.AppReaderRequired;

        // If we have an appid check permissions
        // This will be missing in install-app scenarios and other calls where the app is not set yet
        if (appReader != null)
        {
            if (!zoneSwitchPermissionCheck.Check(appReader, out var error))
                throw HttpException.PermissionDenied(error);
        }

        var cb = uiContextBuilder.InitApp(appReader);

        return new()
        {
            Context = cb.Get(Ctx.General, CtxEnable.All),
        };
    }


}

public class DialogContextStandaloneDto
{
    public required ContextDto Context { get; init; }
}
