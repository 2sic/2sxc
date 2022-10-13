using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the AppInternalsController (Web API Controller)
    /// </summary>

    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]
    public class AppInternalsController : OqtStatefulControllerBase<AppInternalsControllerReal>, IAppInternalsController
    {
        public AppInternalsController() : base(AppInternalsControllerReal.LogSuffix) { }

        /// <inheritdoc/>
        [HttpGet]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public AppInternalsDto Get(int appId, int targetType, string keyType, string key)
            => Real.Get(appId, targetType, keyType, key);
    }
}