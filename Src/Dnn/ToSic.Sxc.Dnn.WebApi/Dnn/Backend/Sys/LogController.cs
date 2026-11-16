using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Logs;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Eav.WebApi.Sys.Logs.LogControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class LogController : DnnApiController, ILogController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create("Api." + RealController.LogSuffix);

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpGet]
    public string EnableDebug(int duration = 1)
        => Real.EnableDebug(DnnLogging.ActivateForDuration, duration);
}