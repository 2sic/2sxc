using System.Web.Http;
using ToSic.Eav.Configuration;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal static class PublicFormsPermissions
    {
        internal static bool UserCanWriteAndPublicFormsEnabled(this MultiPermissionsApp mpa, out HttpResponseException preparedException)
        {
            var wrapLog = mpa.Log.Call("UserCanWriteAndPublicFormsEnabled", "");
            // 1. check if user is restricted
            var userIsRestricted = !mpa.UserMay(GrantSets.WritePublished);

            // 2. check if feature is enabled
            var feats = new[] { FeatureIds.PublicForms };
            if (userIsRestricted && !Features.Enabled(feats))
            {
                preparedException = Http.PermissionDenied($"low-permission users may not access this - {Features.MsgMissingSome(feats)}");
                return false;
            }
            wrapLog("ok");
            preparedException = null;
            return true;
        }

    }
}
