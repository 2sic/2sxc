using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ApiExplorer;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    public class OqtApiInspector : HasLog<IApiInspector>, IApiInspector
    {

        public OqtApiInspector(): base(OqtConstants.OqtLogPrefix)
        {
            
        }

        public bool IsBody(ParameterInfo paramInfo)
        {
            return paramInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(FromBodyAttribute));
        }

        public List<string> GetHttpVerbs(MethodInfo methodInfo)
        {
            var httpMethods = new List<string>();

            var getAtt = methodInfo.GetCustomAttribute<HttpGetAttribute>();
            if (getAtt != null) httpMethods.Add(getAtt.HttpMethods.First());

            var postAtt = methodInfo.GetCustomAttribute<HttpPostAttribute>();
            if (postAtt != null) httpMethods.Add(postAtt.HttpMethods.First());

            var putAtt = methodInfo.GetCustomAttribute<HttpPutAttribute>();
            if (putAtt != null) httpMethods.Add(putAtt.HttpMethods.First());

            var deleteAtt = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
            if (deleteAtt != null) httpMethods.Add(deleteAtt.HttpMethods.First());

            var acceptVerbsAtt = methodInfo.GetCustomAttribute<AcceptVerbsAttribute>();
            if (acceptVerbsAtt != null) httpMethods.AddRange(acceptVerbsAtt.HttpMethods);

            return httpMethods;
        }

        public ApiSecurityDto GetSecurity(MemberInfo member)
        {
            var oqtAuthList = member.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToList();

            return new ApiSecurityDto
            {
                ignoreSecurity = member.GetCustomAttribute<AllowAnonymousAttribute>() != null,
                allowAnonymous = oqtAuthList.Any(a => a.Roles == RoleNames.Everyone),
                requireVerificationToken = member.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() != null || member.GetCustomAttribute<AutoValidateAntiforgeryTokenAttribute>() != null,
                superUser = oqtAuthList.Any(a => a.Roles == RoleNames.Host),
                admin = oqtAuthList.Any(a => a.Roles == RoleNames.Admin),
                //edit = oqtAuthList.Any(a => a.Roles == SecurityAccessLevel.Edit),
                //view = oqtAuthList.Any(a => a.Roles == SecurityAccessLevel.View),
                // if it has any authorize attributes or supported-modules it needs the context
                requireContext = oqtAuthList.Any() /*|| member.GetCustomAttribute<SupportedModulesAttribute>() != null*/,
            };
        }
    }
}
