using System.Reflection;
using ToSic.Eav.WebApi.Sys.ApiExplorer;

namespace ToSic.Sxc.Dnn.Backend.Admin;

internal class DnnApiInspector() : ServiceBase(DnnConstants.LogName), IApiInspector
{
    public bool IsBody(ParameterInfo paramInfo)
        => paramInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(FromBodyAttribute));


    public List<string> GetHttpVerbs(MethodInfo methodInfo)
    {
        var httpMethods = new List<string>();

        var getAtt = methodInfo.GetCustomAttribute<HttpGetAttribute>();
        if (getAtt != null)
            httpMethods.Add(getAtt.HttpMethods[0].Method);

        var postAtt = methodInfo.GetCustomAttribute<HttpPostAttribute>();
        if (postAtt != null)
            httpMethods.Add(postAtt.HttpMethods[0].Method);

        var putAtt = methodInfo.GetCustomAttribute<HttpPutAttribute>();
        if (putAtt != null)
            httpMethods.Add(putAtt.HttpMethods[0].Method);

        var deleteAtt = methodInfo.GetCustomAttribute<HttpDeleteAttribute>();
        if (deleteAtt != null)
            httpMethods.Add(deleteAtt.HttpMethods[0].Method);

        var acceptVerbsAtt = methodInfo.GetCustomAttribute<AcceptVerbsAttribute>();
        if (acceptVerbsAtt != null)
            httpMethods.AddRange(acceptVerbsAtt.HttpMethods.Select(m => m.Method));

        return httpMethods;
    }

    public ApiSecurityDto GetSecurity(MemberInfo member)
    {
        var dnnAuthList = member.GetCustomAttributes<DnnModuleAuthorizeAttribute>().ToList();

        return new()
        {
            IgnoreSecurity = member.GetCustomAttribute<AllowAnonymousAttribute>() != null,
            AllowAnonymous = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Anonymous),
            RequireVerificationToken = member.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() != null,
            SuperUser = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Host),
            Admin = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Admin),
            Edit = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.Edit),
            View = dnnAuthList.Any(a => a.AccessLevel == SecurityAccessLevel.View),
            // if it has any dnn authorize attributes or supported-modules it needs the context
            RequireContext = dnnAuthList.Any() || member.GetCustomAttribute<SupportedModulesAttribute>() != null,
        };
    }
}