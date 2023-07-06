using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.WebApi.Admin.AppPartsControllerReal<Microsoft.AspNetCore.Mvc.IActionResult>;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download


    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    public class AppPartsController : OqtStatefulControllerBase, IAppPartsController<IActionResult>
    {
        public AppPartsController(): base(RealController.LogSuffix) { }

        private RealController Real => GetService<RealController>();


        #region Parts Export/Import

        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => Real.Get(zoneId: zoneId, appId: appId, scope: scope);


        /// <inheritdoc />
        [HttpGet]
        public IActionResult Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);

            return Real.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString, entityIdsString: entityIdsString, templateIdsString: templateIdsString);
        }


        /// <inheritdoc />
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ImportResultDto Import(int zoneId, int appId) 
            => Real.Import(uploadInfo: new(Request), zoneId: zoneId, appId: appId);

        #endregion
    }
}