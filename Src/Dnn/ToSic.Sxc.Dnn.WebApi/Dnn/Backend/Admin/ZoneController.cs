using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Zone;
using RealController = ToSic.Eav.WebApi.Admin.ZoneControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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