using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class PermissionCheckWithEnsure: HasLog, IPermissionCheck
    {
        internal IPermissionCheck Original;

        public PermissionCheckWithEnsure(IPermissionCheck original, Log parentLog):base("", parentLog)
        {
            Original = original;
        }

        public bool HasPermissions => Original.HasPermissions;

        public bool UserMay(List<Grants> grants) => Original.UserMay(grants);

        public ConditionType GrantedBecause => Original.GrantedBecause;

        public bool Ensure(List<Grants> grants, out HttpResponseException exception)
        {
            var wrapLog = Log.Call("Ensure", () => $"[{string.Join(",", grants)}]", () => "or throw");
            var ok = Original.UserMay(grants);
            exception = ok ? null : Http.PermissionDenied("required permissions for this request are not given");
            wrapLog(ok ? "ok" : "permissions not ok");
            return ok;
        }
    }
}
