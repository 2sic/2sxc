using System.Web;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Backend.Admin;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Web API Controller for app data bundles etc.
/// </summary>
/// <remarks>
/// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
/// So we can't use the classic protection attributes to the class like:
/// - [SupportedModules(DnnSupportedModuleNames)]
/// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
/// - [ValidateAntiForgeryToken]
/// Instead, each method must have all attributes, or do additional security checking.
/// Security checking is possible, because the cookie still contains user information
/// </remarks>
[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DataController() : DnnSxcControllerBase(DataControllerReal.LogSuffix), IAdminDataController
{
    private DataControllerReal Real => SysHlp.GetService<DataControllerReal>();

    /// <summary>
    /// Bundle Export
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage BundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.BundleExport(appId, exportConfiguration, indentation);
    }

    /// <summary>
    /// Bundle Import
    /// </summary>
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto BundleImport(int zoneId, int appId)
    {
        SysHlp.PreventServerTimeout300();
        return Real.BundleImport(new(Request, HttpContext.Current.Request), zoneId, appId);
    }

    /// <summary>
    /// Bundle Save
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleSave(int appId, Guid exportConfiguration, int indentation = 0)
        => Real.BundleSave(appId, exportConfiguration, indentation);

    /// <summary>
    /// Bundle Restore
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public bool BundleRestore(string fileName, int zoneId, int appId)
        => Real.BundleRestore(fileName, zoneId, appId);
}