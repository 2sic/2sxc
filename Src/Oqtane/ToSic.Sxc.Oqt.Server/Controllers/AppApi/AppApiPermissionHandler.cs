using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Oqtane.Infrastructure;
using Oqtane.Security;
using System.Threading.Tasks;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    /**
     * Extend Oqtane default PermissionHandler to provide Oqt required "entityId"
     * if missing from header "moduleId", or query string, or route value.
     */
    public class AppApiPermissionHandler : PermissionHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly RequestHelper _requestHelper;

        public AppApiPermissionHandler(IHttpContextAccessor httpContextAccessor, IUserPermissions userPermissions, ILogManager logger, RequestHelper requestHelper) : base(httpContextAccessor, userPermissions, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userPermissions = userPermissions;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!httpContext.Request.Query.ContainsKey("entityid"))
            {
                var moduleId = _requestHelper.GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId,
                    _requestHelper.GetQueryString(WebApiConstants.ModuleId,
                        _requestHelper.GetRouteValuesString(WebApiConstants.ModuleId,
                            -1)));
                var queryString = httpContext.Request.QueryString.Add(new QueryString($"?entityid={moduleId}"));
                httpContext.Request.QueryString = queryString;
            }

            base.HandleRequirementAsync(context, requirement);

            return Task.CompletedTask;
        }
    }
}
