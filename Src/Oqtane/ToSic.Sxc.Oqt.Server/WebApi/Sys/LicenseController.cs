using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Sys.Licenses.LicenseControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/" + AreaRoutes.Sys)]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LicenseController() : OqtStatefulControllerBase("License"), ILicenseController
{
    private RealController Real => GetService<RealController>();



    /// <summary>
    /// Make sure that these requests don't land in the normal api-log.
    /// Otherwise each log-access would re-number what item we're looking at
    /// </summary>
    protected override string HistoryLogGroup { get; } = "web-api.license";

    #region License

    /// <inheritdoc />
    [HttpGet]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    [Authorize(Roles = RoleNames.Host)]
    public IEnumerable<LicenseDto> Summary() => Real.Summary();


    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Host)]
    public LicenseFileResultDto Upload() => Real.Upload(new(Request));


    /// <inheritdoc />
    [HttpGet]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    [Authorize(Roles = RoleNames.Host)]
    public LicenseFileResultDto Retrieve() => Real.Retrieve();

    #endregion

}