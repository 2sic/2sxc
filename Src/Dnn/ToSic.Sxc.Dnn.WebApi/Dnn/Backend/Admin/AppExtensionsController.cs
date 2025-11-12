using System.Text.Json;
using System.Web;
using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.Dto;
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
    public ExtensionsResultDto Extensions(int appId) => Real.Extensions(appId);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [JsonFormatter(Casing = Casing.Camel)]
    public bool Extensions([FromUri] int zoneId, [FromUri] int appId, [FromUri] string name, [FromBody] JsonElement configuration)
        => Real.Extensions(zoneId, appId, name, configuration);

    /// <inheritdoc />
    [HttpPost]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool InstallExtension([FromUri] int zoneId, [FromUri] int appId, [FromUri] bool overwrite = true)
        => Real.InstallExtensionZip(new(Request, HttpContext.Current.Request), zoneId, appId, overwrite);

    /// <inheritdoc />
    [HttpGet]
    // Note: since this is a GET download, there is no ValidateAntiForgeryToken and no header with the module names
    //[ValidateAntiForgeryToken]
    //[SupportedModules(DnnSupportedModuleNames)]
    [DnnAuthorize(StaticRoles = "Administrators")]
    public HttpResponseMessage Download([FromUri] int zoneId, [FromUri] int appId, [FromUri] string name)
        => Real.Download(zoneId, appId, name);
}
