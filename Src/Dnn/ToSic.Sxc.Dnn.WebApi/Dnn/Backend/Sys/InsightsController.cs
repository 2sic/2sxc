using RealController = ToSic.Eav.WebApi.Sys.Insights.InsightsControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class InsightsController() : DnnSxcControllerRoot(RealController.LogSuffix)
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <summary>
    /// Single-Point-Of-Entry
    /// The insights handle all their work in the backend, incl. view-switching.
    /// This is important for many reasons, inc. the fact that this will always be the first endpoint to implement
    /// on any additional system. 
    /// </summary>
    [HttpGet] // Will do security checks internally
    public string Details(string view, int? appId = null, string key = null, int? position = null, string type = null, bool? toggle = null, string nameId = null, string filter = default)
        => Real.Details(view, appId, key, position, type, toggle, nameId, filter);


    #region Controll Logging of Requests on Insights for special debugging, usually disabled to not clutter the logs

    /// <summary>
    /// Enable/disable logging of access to insights
    /// Only enable this if you have trouble developing insights, otherwise it clutters our logs
    /// </summary>
    internal static bool InsightsLoggingEnabled = false;

    /// <summary>
    /// Special detection for the AppApiController to skip these requests?
    /// </summary>
    internal const string InsightsUrlFragment = "/sys/insights/";

    /// <summary>
    /// Make sure that these requests don't land in the normal api-log.
    /// Otherwise each log-access would re-number what item we're looking at
    /// </summary>
    protected override string HistoryLogGroup => "web-api.insights";

    #endregion
}