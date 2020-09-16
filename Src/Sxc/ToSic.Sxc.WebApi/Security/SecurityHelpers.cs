using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.WebApi.Security
{
    public static class SecurityHelpers
    {
        internal static void ThrowIfNotEditorOrIsPublicForm(IInstanceContext context, Sxc.Apps.IApp app, string contentTypeStaticName, ILog log)
        {
            var permCheck = new MultiPermissionsTypes().Init(context, app, contentTypeStaticName, log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);

            if (!permCheck.UserCanWriteAndPublicFormsEnabled(out _, out error))
                throw HttpException.PermissionDenied(error);
        }

    }
}
