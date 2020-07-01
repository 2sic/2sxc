using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Security;

namespace ToSic.Sxc.Security
{
    internal interface IMultiPermissionCheck
    {
        bool UserMayOnAll(List<Grants> grants);

        bool EnsureAll(List<Grants> grants, out HttpResponseException preparedException);

    }
}
