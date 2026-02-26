using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend;
internal class ApiForBlockHelpers
{
    public static void ThrowIfNotAllowedInApp(Generator<MultiPermissionsApp, MultiPermissionsApp.Options> multiPermissionsApp, IContextOfBlock context, List<Grants> requiredGrants, IAppIdentity? alternateApp = null)
    {
        var permCheck = multiPermissionsApp.New(new() { Context = context, App = alternateApp ?? context.AppReaderRequired });
        if (!permCheck.EnsureAll(requiredGrants, out var error))
            throw HttpException.PermissionDenied(error);
    }

}
