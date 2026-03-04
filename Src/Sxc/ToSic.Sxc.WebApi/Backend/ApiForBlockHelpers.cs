using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend;
internal static class ApiForBlockHelpers
{
    public static void ThrowIfNotAllowedInApp(this Generator<MultiPermissionsApp, MultiPermissionsApp.Options> multiPermissionsApp,
        IContextOfBlock context,
        List<Grants> requiredGrants,
        IAppIdentity? alternateApp = null)
    {
        var permCheck = multiPermissionsApp.New(new() { SiteContext = context, App = alternateApp ?? context.AppReaderRequired });
        if (!permCheck.EnsureAll(requiredGrants, out var error))
            throw HttpException.PermissionDenied(error);
    }

}
