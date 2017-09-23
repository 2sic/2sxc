using System;
using System.Collections.Concurrent;
using System.Web.Http;
using DotNetNuke.Services.Log.EventLog;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public static class Logging
    {
        public const int MaxDuration = 10;

        public static void LogToDnn(string key, string message, Log log = null, DnnHelper dnnContext = null, bool force = false)
        {
            if(!force)
                if (CheckIfWeShouldSkipEventLogging(GlobalConfiguration.Configuration.Properties)) return;


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

                log?.Entries.ForEach(e => logInfo.AddProperty(e.Source, e.Message));

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
            catch
            {
            }
        }

        private static void AttachDnnStateIfPossible(DnnHelper dnn, LogInfo logInfo)
        {
            try
            {
                if (dnn != null)
                {
                    logInfo.LogUserName = dnn.User?.DisplayName ?? "unknown";
                    logInfo.LogUserID = dnn.User?.UserID ?? -1;
                    logInfo.LogPortalID = dnn.Portal.PortalId;
                    logInfo.AddProperty("ModuleId", dnn.Module?.ModuleID.ToString() ?? "unknown");
                }
            }
            catch
            {
            }
        }

        public static bool CheckIfWeShouldSkipEventLogging(ConcurrentDictionary<object, object> props)
        {
            if (props == null) return true;
            if (!props.TryGetValue(Constants.AdvancedLoggingEnabledKey, out var enabled)) return true;
            if (!(enabled is bool)) return true;
            if (!(bool)enabled) return true;

            if (!props.TryGetValue(Constants.AdvancedLoggingTillKey, out var till)) return true;
            if (!(till is DateTime)) return true;
            var dtmTill = (DateTime)till;
            if (dtmTill.CompareTo(DateTime.Now) <= 0) return true;
            return false;
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
            prop.GetOrAdd(Constants.AdvancedLoggingEnabledKey, duration > 0);
            var timeout = DateTime.Now.AddMinutes(duration);
            prop.AddOrUpdate(Constants.AdvancedLoggingTillKey, timeout, (a, b) => timeout);
            return $"Extended logging activated for {duration} minutes to {timeout}";
        }
    }
}