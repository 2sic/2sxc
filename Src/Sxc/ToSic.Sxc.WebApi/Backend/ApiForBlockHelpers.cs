using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend;
internal class ApiForBlockHelpers
{
    public static void ThrowIfNotAllowedInApp(Generator<MultiPermissionsApp> multiPermissionsApp, IContextOfBlock context, List<Grants> requiredGrants, IAppIdentity? alternateApp = null)
    {
        var permCheck = multiPermissionsApp.New().Init(context, alternateApp ?? context.AppReaderRequired);
        if (!permCheck.EnsureAll(requiredGrants, out var error))
            throw HttpException.PermissionDenied(error);
    }

}
