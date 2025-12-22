using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps.Sys.Work;
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


[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataController() : OqtStatefulControllerBase(DataControllerReal.LogSuffix), IAdminDataController
{
    private DataControllerReal Real => GetService<DataControllerReal>();

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult BundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        CtxHlp.SetupResponseMaker();
        return Real.BundleExport(appId, exportConfiguration, indentation);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResultDto BundleImport(int zoneId, int appId)
        => Real.BundleImport(new(Request), zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0)
        => Real.BundleSave(appId, exportConfiguration, indentation);

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleRestore(string fileName, int zoneId, int appId)
        => Real.BundleRestore(fileName, zoneId, appId);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Host)]
    public IReadOnlyList<WorkEntityRecycleBin.RecycleBinItem> GetRecycleBin(int appId)
        => Real.GetRecycleBin(appId);

    /// <inheritdoc />
    [HttpGet]
    [Authorize(Roles = RoleNames.Host)]
    public void Recycle(int appId, int transactionId)
        => Real.Recycle(appId, transactionId);
}