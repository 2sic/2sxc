using System.Web;
using ToSic.Eav.Apps.Sys.Work;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Sys;

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
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataController() : DnnSxcControllerBase(DataControllerReal.LogSuffix), IAdminDataController
{
    private DataControllerReal Real => SysHlp.GetService<DataControllerReal>();

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage BundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.BundleExport(appId, exportConfiguration, indentation);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto BundleImport(int zoneId, int appId)
    {
        SysHlp.PreventServerTimeout600();
        return Real.BundleImport(new(Request, HttpContext.Current.Request), zoneId, appId);
    }

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
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public IReadOnlyList<WorkEntityRecycleBin.RecycleBinItem> GetRecycleBin(int appId)
        => Real.GetRecycleBin(appId);


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public void Recycle(int appId, int transactionId)
        => Real.Recycle(appId, transactionId);

}