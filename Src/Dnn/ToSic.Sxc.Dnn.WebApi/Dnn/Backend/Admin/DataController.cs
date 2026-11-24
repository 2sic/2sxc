using System.Web;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.DataControllerReal;
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
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataController : DnnApiController, IAdminDataController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create("Api." + RealController.LogSuffix);

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion

    private RealController Real => SysHlp.GetServiceNoExCtx<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage BundleExport(int appId, Guid exportConfiguration, int indentation = 0)
        => Real.BundleExport(appId, exportConfiguration, indentation).ToHttpResponse();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResult BundleImport(int zoneId, int appId)
    {
        this.PreventServerTimeout600();
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
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public void Recycle(int appId, int transactionId)
        => Real.Recycle(appId, transactionId);

}