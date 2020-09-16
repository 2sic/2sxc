using System;
using System.Security.Authentication;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;

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


        internal static T RunIfAdmin<T>(IUser user, Func<T> task)
        {
            ThrowIfNotAdmin(user);
            return task.Invoke();
        }

        internal static void ThrowIfNotAdmin(IUser user)
        {
            if (!user.IsAdmin)
                throw new AuthenticationException("Needs admin permissions to do this");
        }

    }
}
