using System.Web;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Admin.AppExtensionsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogExceptions]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppExtensionsController() : DnnSxcControllerBase(RealController.LogSuffix), IAppExtensionsController<HttpResponseMessage>
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public ExtensionsResultDto Extensions(int appId)
        => Real.Extensions(appId);

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
        SysHlp.PreventServerTimeout600();
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
        SysHlp.PreventServerTimeout600();
        return Real.InstallFrom(urls, zoneId, appId, editions, overwrite);
    }

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public ExtensionInspectResultDto Inspect(int appId, string name, string edition = null)
        => Real.Inspect(appId, name, edition);

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

    ///// <summary>
    ///// Alias POST endpoint for front-ends posting to /appExtensions/extensions with query parameters.
    ///// Matches plural POST behavior to avoid 405 errors if client uses POST.
    ///// </summary>
    ////[ActionName("extensions")]
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //[SupportedModules(DnnSupportedModuleNames)]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    //[JsonFormatter(Casing = Casing.Camel)]
    //public bool Extensions(int appId, string name, [FromBody] ExtensionManifest configuration)
    //    => Real.Extension(appId, name, configuration);

    /// <inheritdoc />
    [HttpGet]
    // Note: since this is a GET download, there is no ValidateAntiForgeryToken and no header with the module names
    //[ValidateAntiForgeryToken]
    //[SupportedModules(DnnSupportedModuleNames)]
    [DnnAuthorize(StaticRoles = "Administrators")]
    public HttpResponseMessage Download(int appId, string name)
        => Real.Download(appId, name);

    /// <inheritdoc />
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public bool Delete(int zoneId, int appId, string name, string edition = null, bool force = false, bool withData = false)
        => Real.Delete(zoneId, appId, name, edition, force, withData);
}
