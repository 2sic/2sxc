using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.WebApi.Admin.AppPartsControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;
// [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
// token, which fails in the cases where the url is called using get, which should result in a download

// Release routes
[Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppPartsController : OqtStatefulControllerBase, IAppPartsController
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
        CtxHlp.SetupResponseMaker();
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