using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal interface IMultiPermissionCheck
    {
        bool UserMayOnAll(List<Grants> grants);

        bool EnsureAll(List<Grants> grants, out HttpResponseException preparedException);

        bool SameAppOrIsSuperUserAndEnsure(List<Grants> grants, out HttpResponseException preparedException);
    }
}
