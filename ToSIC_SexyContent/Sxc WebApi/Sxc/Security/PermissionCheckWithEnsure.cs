using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.WebApi;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal static class PermissionCheckWithEnsure 
    {
        /// <summary>
        /// Run a permission check and return error if it failed
        /// </summary>
        /// <param name="permCheck"></param>
        /// <param name="grants"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool Ensure(this IPermissionCheck permCheck, List<Grants> grants, out HttpResponseException exception)
        {
            var wrapLog = permCheck.Log.Call("Ensure", () => $"[{string.Join(",", grants)}]", () => "or throw");
            var ok = permCheck.UserMay(grants);
            exception = ok ? null : Http.PermissionDenied("required permissions for this request are not given");
            wrapLog(ok ? "ok" : "permissions not ok");
            return ok;
        }
    }
}
