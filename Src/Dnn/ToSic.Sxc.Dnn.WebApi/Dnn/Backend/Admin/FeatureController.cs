using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using ToSic.Sys.Capabilities.Features;
using RealController = ToSic.Eav.WebApi.Admin.Sys.FeatureControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Provide information about activated features which will be managed externally. 
/// </summary>
/// <remarks>
/// Added in 2sxc 10
/// </remarks>
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FeatureController : DnnApiController, IFeatureController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();
    
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public FeatureStateDto Details(string nameId)
        => Real.Details(nameId);

    /// <summary>
    /// POST updated features JSON configuration.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 13
    /// </remarks>
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool SaveNew([FromBody] List<FeatureStateChange> changes)
        => Real.SaveNew(changes);
}
