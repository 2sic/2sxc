using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.WebApi.Security
{
    internal static class PublicFormsPermissions
    {
        internal static bool UserCanWriteAndPublicFormsEnabled(this MultiPermissionsApp mpa, out HttpExceptionAbstraction preparedException, out string error)
        {
            var wrapLog = mpa.Log.Call("");
            // 1. check if user is restricted
            var userIsRestricted = !mpa.UserMayOnAll(GrantSets.WritePublished);

            // 2. check if feature is enabled
            var feats = new[] { FeatureIds.PublicForms };
            if (userIsRestricted && !ToSic.Eav.Configuration.Features.Enabled(feats))
            {
                error = $"low-permission users may not access this - {ToSic.Eav.Configuration.Features.MsgMissingSome(feats)}";
                preparedException = HttpException.PermissionDenied(error);
                return false;
            }
            wrapLog("ok");
            preparedException = null;
            error = null;
            return true;
        }

    }
}
