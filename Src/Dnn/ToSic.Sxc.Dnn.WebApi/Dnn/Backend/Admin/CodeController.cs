using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.CodeControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeController : DnnApiController, IHasLog
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion

    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<RealController.HelpItem> InlineHelp(string language)
        => Real.InlineHelp(language);

    [HttpGet]
    [ValidateAntiForgeryToken]
    [SupportedModules(DnnSupportedModuleNames)]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public RichResult GenerateDataModels(int appId, string generator, string edition = null, int configurationId = 0)
        => Real.GenerateDataModels(appId, edition, generator: generator, configurationId: configurationId);
}
