using System.Collections.Concurrent;
using System.Web.Http;
using DotNetNuke.Services.Log.EventLog;

namespace ToSic.Sxc.Dnn.Run;

internal static class DnnLogging
{
    public const int MaxDuration = 10;

    public static void LogToDnn(string key, string message, ILog log = null, IDnnContext dnnContext = null, bool force = false)
    {
        if (!force || !EnableLogging(GlobalConfiguration.Configuration.Properties)) return;

        // note: this code has a lot of try/catch, to ensure that most of it works and that
        // it doesn't interfere with other functionality
        try
        {
            var logInfo = new LogInfo
            {
                LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString()
            };

            // the initial message should come first, as it's visible in the summary
            if(!string.IsNullOrEmpty(message))
                logInfo.AddProperty(key, message);

            AttachDnnStateIfPossible(dnnContext, logInfo);

            (log as Log)?.Entries.ForEach(e => logInfo.AddProperty(e.Source, e.Message));

            new EventLogController().AddLog(logInfo);
        }
        catch
        {
            TryToReportLoggingFailure("logging");
        }
    }

    /// <summary>
    /// try to at least report, that something failed
    /// </summary>
    /// <param name="source"></param>
    public static void TryToReportLoggingFailure(string source)
    {
        try
        {
            new EventLogController().AddLog("2sxc logging",
                $"failed to add log from {source}, something in the logging failed", EventLogController.EventLogType.ADMIN_ALERT);
        }
        catch { /* ignore */ }
    }

    private static void AttachDnnStateIfPossible(IDnnContext dnn, LogInfo logInfo)
    {
        try
        {
            if (dnn != null)
            {
                logInfo.LogUserName = dnn.User?.DisplayName ?? Eav.Constants.NullNameId;
                logInfo.LogUserID = dnn.User?.UserID ?? -1;
                logInfo.LogPortalID = dnn.Portal.PortalId;
                logInfo.AddProperty("Module Id", dnn.Module?.ModuleID.ToString() ?? Eav.Constants.NullNameId);
            }
        }
        catch { /* ignore */ }
    }

    public static bool EnableLogging(ConcurrentDictionary<object, object> props)
    {
        if (props == null) return false;
        if (!props.TryGetValue(DnnConstants.AdvancedLoggingEnabledKey, out var enabled)) return false;
        if (enabled is not true) return false;

        if (!props.TryGetValue(DnnConstants.AdvancedLoggingTillKey, out var till)) return false;
        return till is DateTime dtmTill && dtmTill.CompareTo(DateTime.Now) > 0;
    }

    /// <summary>
    /// Activate extended logging for a specific duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static string ActivateForDuration(int duration)
    {
        if (duration > MaxDuration)
            duration = MaxDuration;

        var prop = GlobalConfiguration.Configuration.Properties;
        prop.GetOrAdd(DnnConstants.AdvancedLoggingEnabledKey, duration > 0);
        var timeout = DateTime.Now.AddMinutes(duration);
        prop.AddOrUpdate(DnnConstants.AdvancedLoggingTillKey, timeout, (_, _) => timeout);
        return $"Extended logging activated for {duration} minutes to {timeout}";
    }
}