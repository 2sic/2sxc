using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal interface IContextPermissionCheck
    {
        bool Ensure(List<Grants> grants, out HttpResponseException preparedException);

    }
}
