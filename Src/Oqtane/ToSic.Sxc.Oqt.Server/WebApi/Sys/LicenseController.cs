using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Eav.WebApi.Sys.Licenses.LicenseControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Sys;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathOrLang + "/" + AreaRoutes.Sys)]
[Route(OqtWebApiConstants.ApiRootPathAndLang + "/" + AreaRoutes.Sys)]

[OqtApiRequest("web-api.license")]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LicenseController : ControllerBase, IHasLog, ILicenseController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();



    #region License

    // Replaced by DataSources System.Licenses and System.FeatureStates through query System.SysData.
    ///// <inheritdoc />
    //[HttpGet]
    ////[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    //[Authorize(Roles = RoleNames.Host)]
    //public IEnumerable<LicenseDto> Summary() => Real.Summary();


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