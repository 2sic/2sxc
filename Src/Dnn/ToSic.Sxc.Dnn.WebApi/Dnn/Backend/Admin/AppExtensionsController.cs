using System.Web;
using ToSic.Eav.Apps.Sys.Extensions;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Apps.Extensions.Sys.Preflight;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.AppExtensionsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsController : DnnApiController, IAppExtensionsController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetServiceNoExCtx<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public PreflightResultDto InstallPreflight(int appId, string editions = "")
        => Real.InstallPreflight(new(Request, HttpContext.Current.Request), appId, editions);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public PreflightResultDto InstallPreflightFrom(int appId, [FromBody] string[] urls, string editions = "")
    {
        this.PreventServerTimeout600();
        return Real.InstallPreflightFrom(urls, appId, editions);
    }

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Install(int zoneId, int appId, string editions = "", bool overwrite = false)
        => Real.Install(new(Request, HttpContext.Current.Request), zoneId, appId, editions, overwrite);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool InstallFrom(int zoneId, int appId, [FromBody] string[] urls, string editions = "", bool overwrite = false)
    {
        this.PreventServerTimeout600();
        return Real.InstallFrom(urls, zoneId, appId, editions, overwrite);
    }


    /// <inheritdoc />
    /// Update/create endpoint using PUT with name as route segment.
    [Route("api/2sxc/admin/[controller]/{name}")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public new bool Configuration(int appId, [FromUri] string name, [FromBody] ExtensionManifest configuration)
        => Real.Configuration(appId, name, configuration);

    /// <inheritdoc />
    [HttpGet]
    // Note: since this is a GET download, there is no ValidateAntiForgeryToken and no header with the module names
    //[ValidateAntiForgeryToken]
    //[SupportedModules(DnnSupportedModuleNames)]
    [DnnAuthorize(StaticRoles = "Administrators")]
    public HttpResponseMessage Download(int appId, string name)
        => Real.Download(appId, name).ToHttpResponse();

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public bool Delete(int appId, string name, string edition = null, bool force = false, bool withData = false)
        => Real.Delete(appId, name, edition, force, withData);
}
