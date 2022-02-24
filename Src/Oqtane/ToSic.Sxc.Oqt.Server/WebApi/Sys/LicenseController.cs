using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.WebApi.Licenses;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
    [Route(WebApiConstants.ApiRootPathNdLang + "/" + AreaRoutes.Sys)]

    public class LicenseController : OqtStatefulControllerBase<LicenseControllerReal>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public LicenseController(): base("License") { }


        /// <summary>
        /// Make sure that these requests don't land in the normal api-log.
        /// Otherwise each log-access would re-number what item we're looking at
        /// </summary>
        protected override string HistoryLogGroup { get; } = "web-api.license";

        #region License

        [HttpGet]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [Authorize(Roles = RoleNames.Host)]
        public IEnumerable<LicenseDto> Summary() => Real.Summary();

        #endregion
       
    }
}
