using ToSic.Eav.WebApi.Sys.Admin.Features;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sys.Capabilities.Features;
using RealController = ToSic.Eav.WebApi.Sys.Admin.Features.FeatureControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Provide information about activated features which will be managed externally. 
/// </summary>
/// <remarks>
/// Added in 2sxc 10
/// </remarks>
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class FeatureController() : DnnSxcControllerRoot(RealController.LogSuffix), IFeatureController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public FeatureStateDto Details(string nameId) => Real.Details(nameId);

    /// <summary>
    /// POST updated features JSON configuration.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 13
    /// </remarks>
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool SaveNew([FromBody] List<FeatureStateChange> changes) => Real.SaveNew(changes);
}