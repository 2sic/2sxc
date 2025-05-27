using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Security.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sys.Code.Help;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineCheckTemplate(LazySvc<AppPermissionCheck> appPermCheckLazy)
    : ServiceBase("Sxc.EngChk", connect: [appPermCheckLazy])
{
    /// <summary>
    /// Template Exceptions like missing configuration or defined type not found
    /// </summary>
    /// <exception cref="RenderingException"></exception>
    internal void CheckExpectedTemplateErrors(IView view, IAppReadContentTypes appState)
    {
        if (view == null)
            throw new RenderingException(ErrHelpConfigMissing);

        if (appState == null)
            throw new RenderingException(ErrHelpConfigMissing, "AppState is null");

        if (view.ContentType != "" && appState.GetContentType(view.ContentType) == null)
            throw new RenderingException(ErrHelpTypeMissing);
    }

    private static readonly CodeHelp ErrHelpConfigMissing = new(name: "Template Config missing", detect: "",
        linkCode: "err-view-config-missing", uiMessage: "Template Configuration Missing");

    private static readonly CodeHelp ErrHelpTypeMissing = new(name: "Content Type Missing", detect: "", linkCode: "err-view-type-missing",
        uiMessage: "The contents of this module cannot be displayed because I couldn't find the assigned content-type.");



    internal void ThrowIfViewPermissionsDenyAccess(IView view, IContextOfApp appContext)
    {
        // do security check IF security exists
        // should probably happen somewhere else - so it doesn't throw errors when not even rendering...
        var templatePermissions = appPermCheckLazy.Value
            .ForItem(appContext, appContext.AppReader, view.Entity);

        // Views only use permissions to prevent access, so only check if there are any configured permissions
        if (appContext.User.IsSiteAdmin || !templatePermissions.HasPermissions)
            return;

        if (!templatePermissions.UserMay(GrantSets.ReadSomething).Allowed)
            // TODO: maybe create an exception which inherits from UnauthorizedAccess - in case this improves behavior / HTTP response
            throw new RenderingException(ErrorHelpNotAuthorized, new UnauthorizedAccessException(
                $"{ErrorHelpNotAuthorized.UiMessage} See {ErrorHelpNotAuthorized.LinkCode}"));
    }

    private static readonly CodeHelp ErrorHelpNotAuthorized = new(name: "Not authorized", detect: "",
        linkCode: "http://2sxc.org/help?tag=view-permissions",
        uiMessage: "This view is not accessible for the current user. To give access, change permissions in the view settings.");
}