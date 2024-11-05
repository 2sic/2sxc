using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// Web API Controller for app data bundles etc.
/// </summary>
//[SupportedModules("2sxc,2sxc-app")]
//[DnnLogExceptions]
[Authorize(Roles = RoleNames.Admin)]
[AutoValidateAntiforgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]


[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DataController() : OqtStatefulControllerBase(DataControllerReal.LogSuffix), IAdminDataController
{
    private DataControllerReal Real => GetService<DataControllerReal>();

    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult BundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.BundleExport(appId, exportConfiguration, indentation);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto BundleImport(int zoneId, int appId)
        => Real.BundleImport(new(Request), zoneId, appId);

    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0)
        => Real.BundleSave(appId, exportConfiguration, indentation);

    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleRestore(string fileName, int zoneId, int appId)
        => Real.BundleRestore(fileName, zoneId, appId);
}