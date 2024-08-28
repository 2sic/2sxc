using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using RealController = ToSic.Eav.WebApi.Admin.AppInternalsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Proxy Class to the AppInternalsController (Web API Controller)
/// </summary>
[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppInternalsController() : DnnSxcControllerBase(RealController.LogSuffix), IAppInternalsController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc/>
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public AppInternalsDto Get(int appId) => Real.Get(appId);
}