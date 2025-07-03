using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Eav.WebApi.Sys.Zone;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Eav.WebApi.Sys.Admin.ZoneControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ZoneController() : DnnSxcControllerBase(RealController.LogSuffix), IZoneController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    public IList<SiteLanguageDto> GetLanguages() => Real.GetLanguages();

    /// <inheritdoc />
    [HttpGet]
    public void SwitchLanguage(string cultureCode, bool enable) => Real.SwitchLanguage(cultureCode, enable);

    /// <inheritdoc />
    [HttpGet]
    public SystemInfoSetDto GetSystemInfo() => Real.GetSystemInfo();
}