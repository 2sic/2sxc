using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.Zone;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Eav.WebApi.Sys.Admin.ZoneControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ZoneController() : DnnSxcControllerBase(RealController.LogSuffix)
{
    private RealController Real => SysHlp.GetService<RealController>();

    ///// <inheritdoc />
    //[HttpGet]
    //public IList<SiteLanguageDto> GetLanguages()
    //    => Real.GetLanguages();

    /// <inheritdoc />
    [HttpGet]
    public void SwitchLanguage(string cultureCode, bool enable)
        => Real.SwitchLanguage(cultureCode, enable);

    /// <inheritdoc />
    /// Replaced by "System.SystemInfo" DataSource; endpoint disabled.
    //[HttpGet]
    //public SystemInfoSetDto GetSystemInfo()
    //    => Real.GetSystemInfo();
}