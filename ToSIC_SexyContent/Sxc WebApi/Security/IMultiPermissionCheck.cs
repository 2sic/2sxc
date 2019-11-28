using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;

namespace ToSic.Sxc.Security
{
    internal interface IMultiPermissionCheck
    {
        bool UserMayOnAll(List<Grants> grants);

        bool EnsureAll(List<Grants> grants, out HttpResponseException preparedException);

        //bool SameAppOrIsSuperUserAndEnsure(List<Grants> grants, out HttpResponseException preparedException);
    }
}
