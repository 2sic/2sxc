using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Security;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;

/// <summary>
/// Extend Oqtane default PermissionHandler to provide Oqt required "entityId"
/// if missing from header "moduleId", or query string, or route value.
/// </summary>
[PrivateApi]
internal class AppApiPermissionHandler : PermissionHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RequestHelper _requestHelper;

    public AppApiPermissionHandler(IHttpContextAccessor httpContextAccessor, IUserPermissions userPermissions, ILogManager logger, RequestHelper requestHelper) : base(httpContextAccessor, userPermissions, logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _requestHelper = requestHelper;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (!httpContext.Request.Query.ContainsKey("entityid"))
        {
            var moduleId = _requestHelper.GetTypedHeader(ContextConstants.ModuleIdKey,
                _requestHelper.GetQueryString(ContextConstants.ModuleIdKey,
                    _requestHelper.GetRouteValuesString(ContextConstants.ModuleIdKey,
                        -1)));
            var queryString = httpContext.Request.QueryString.Add(new($"?entityid={moduleId}"));
            httpContext.Request.QueryString = queryString;
        }

        base.HandleRequirementAsync(context, requirement);

        return Task.CompletedTask;
    }
}