using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Backend.Admin;
using RealController = ToSic.Sxc.Backend.Admin.CodeControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeController() : DnnSxcControllerBase(RealController.LogSuffix)
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<RealController.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);

    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public RichResult GenerateDataModels(int appId, string generator, string edition = default) => Real.GenerateDataModels(appId, edition, generator: generator);

    [HttpGet]
    [JsonFormatter]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public EditionsDto GetEditions(int appId) => Real.GetEditions(appId);
}