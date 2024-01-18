using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Dnn.Run;
using RealController = ToSic.Eav.WebApi.Sys.LogControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[DnnLogExceptions]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LogController() : DnnSxcControllerRoot(RealController.LogSuffix), ILogController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    public string EnableDebug(int duration = 1) => Real.EnableDebug(DnnLogging.ActivateForDuration, duration);
}