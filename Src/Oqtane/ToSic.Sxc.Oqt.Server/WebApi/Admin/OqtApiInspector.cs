using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Reflection;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtApiInspector() : ServiceBase(OqtConstants.OqtLogPrefix), IApiInspector
{
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
        var oqtAuthList = member.GetCustomAttributes<AuthorizeAttribute>().ToList();

        return new()
        {
            ignoreSecurity = member.GetCustomAttribute<AllowAnonymousAttribute>() != null,
            allowAnonymous = oqtAuthList.Any(a => a.Roles == RoleNames.Everyone),

            requireVerificationToken = GetRequireVerificationToken(member),
            _validateAntiForgeryToken = member.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>() != null,
            _autoValidateAntiforgeryToken = member.GetCustomAttribute<AutoValidateAntiforgeryTokenAttribute>() != null,
            _ignoreAntiforgeryToken = member.GetCustomAttribute<IgnoreAntiforgeryTokenAttribute>() != null,

            superUser = oqtAuthList.Any(a => a.Roles == RoleNames.Host),
            admin = oqtAuthList.Any(a => a.Roles == RoleNames.Admin),
            edit = oqtAuthList.Any(a => a.Policy == PolicyNames.EditModule),
            view = oqtAuthList.Any(a => a.Policy == PolicyNames.ViewModule),
                
            // Supported-modules attribute do not exist in Oqtane
            requireContext = oqtAuthList.Any(),
        };
    }

    /// AntiForgery token validation is enabled or skipped based on:
    /// - ValidateAntiForgeryToken attribute
    /// - AutoValidateAntiforgeryToken attribute for unsafe HTTP methods
    /// - IgnoreAntiforgeryToken attribute
    private bool GetRequireVerificationToken(MemberInfo member)
    {
        var (validateAntiForgeryToken, autoValidateAntiforgeryToken, ignoreAntiforgeryToken) = GetLastAntiForgeryTokenAttribute(member);

        // ValidateAntiForgeryToken attribute enables AntiForgery token validation,
        // AutoValidateAntiforgeryToken attribute enables AntiForgery token validation for all unsafe HTTP methods.
        // IgnoreAntiforgeryToken attribute skips AntiForgery token validation.
        return !ignoreAntiforgeryToken && (validateAntiForgeryToken || (autoValidateAntiforgeryToken && IsUnsafeHttpMethod(member)));
    }

    private static (bool validateAntiForgeryToken, bool autoValidateAntiforgeryToken, bool ignoreAntiforgeryToken)
        GetLastAntiForgeryTokenAttribute(MemberInfo member)
    {
        // All AntiForgeryToken attributes have the same attribute order (Order=1000).
        // In practice only one AntiForgeryToken attribute should be used per member.
        // If we have more than one, only the last AntiForgeryToken attribute prevails (other are ignored).
        // This is the reason why we try to find only last of the AntiForgeryToken attributes on member.
        var validateAntiForgeryToken = false;
        var autoValidateAntiforgeryToken = false;
        var ignoreAntiforgeryToken = false;

        var orderedList = member.CustomAttributes.ToList();
        for (var i = orderedList.Count - 1; i > 0; i--)
        {
            if (orderedList[i].AttributeType == typeof(ValidateAntiForgeryTokenAttribute))
            {
                validateAntiForgeryToken = true;
                break;
            }

            if (orderedList[i].AttributeType == typeof(AutoValidateAntiforgeryTokenAttribute))
            {
                autoValidateAntiforgeryToken = true;
                break;
            }

            if (orderedList[i].AttributeType == typeof(IgnoreAntiforgeryTokenAttribute))
            {
                ignoreAntiforgeryToken = true;
                break;
            }
        }

        return (validateAntiForgeryToken, autoValidateAntiforgeryToken, ignoreAntiforgeryToken);
    }

    // AutoValidateAntiforgeryTokenAttribute causes validation of antiforgery tokens for all unsafe HTTP methods.
    // An antiforgery token is required for HTTP methods other than GET, HEAD, OPTIONS and TRACE.
    private bool IsUnsafeHttpMethod(MemberInfo member)
    {
        // HttpVerbs cant be on class, so just return true.
        if (member is not MethodInfo method) return true;
        // Find unsafe HTTP methods.
        var safeHttpMethods = new List<string>() { "GET", "HEAD", "OPTIONS", "TRACE" };
        var httpVerbs = GetHttpVerbs(method);
        httpVerbs.RemoveAll(t => safeHttpMethods.Contains(t));
        return httpVerbs.Any();
    }
}