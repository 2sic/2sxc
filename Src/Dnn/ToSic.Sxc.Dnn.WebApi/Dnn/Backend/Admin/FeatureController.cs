using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;
using ToSic.Eav.WebApi.Admin.Features;
using RealController = ToSic.Eav.WebApi.Admin.Features.FeatureControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Provide information about activated features which will be managed externally. 
/// </summary>
/// <remarks>
/// Added in 2sxc 10
/// </remarks>
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FeatureController() : DnnSxcControllerRoot(RealController.LogSuffix), IFeatureController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public FeatureState Details(string nameId) => Real.Details(nameId);

    /// <summary>
    /// POST updated features JSON configuration.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 13
    /// </remarks>
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool SaveNew([FromBody] List<FeatureManagementChange> changes) => Real.SaveNew(changes);
}