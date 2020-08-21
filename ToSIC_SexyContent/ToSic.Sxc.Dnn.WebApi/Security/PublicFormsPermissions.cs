using System.Web.Http;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Security
{
    internal static class PublicFormsPermissions
    {
        internal static bool UserCanWriteAndPublicFormsEnabled(this MultiPermissionsApp mpa, out HttpResponseException preparedException, out string error)
        {
            var wrapLog = mpa.Log.Call("");
            // 1. check if user is restricted
            var userIsRestricted = !mpa.UserMayOnAll(GrantSets.WritePublished);

            // 2. check if feature is enabled
            var feats = new[] { FeatureIds.PublicForms };
            if (userIsRestricted && !Features.Enabled(feats))
            {
                error = $"low-permission users may not access this - {Features.MsgMissingSome(feats)}";
                preparedException = Http.PermissionDenied(error);
                return false;
            }
            wrapLog("ok");
            preparedException = null;
            error = null;
            return true;
        }

    }
}
