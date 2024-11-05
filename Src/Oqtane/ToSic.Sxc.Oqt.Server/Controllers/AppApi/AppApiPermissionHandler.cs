using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Security;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

/// <summary>
/// Extend Oqtane default PermissionHandler to provide Oqt required "entityId"
/// if missing from header "moduleId", or query string, or route value.
/// </summary>
[PrivateApi]
internal class AppApiPermissionHandler(
    IHttpContextAccessor httpContextAccessor,
    IUserPermissions userPermissions,
    ILogManager logger,
    RequestHelper requestHelper)
    : PermissionHandler(httpContextAccessor, userPermissions, logger)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (!httpContext.Request.Query.ContainsKey("entityid"))
        {
            var moduleId = requestHelper.GetTypedHeader(ContextConstants.ModuleIdKey,
                requestHelper.GetQueryString(ContextConstants.ModuleIdKey,
                    requestHelper.GetRouteValuesString(ContextConstants.ModuleIdKey,
                        -1)));
            var queryString = httpContext.Request.QueryString.Add(new($"?entityid={moduleId}"));
            httpContext.Request.QueryString = queryString;
        }

        base.HandleRequirementAsync(context, requirement);

        return Task.CompletedTask;
    }
}