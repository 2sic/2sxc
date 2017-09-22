using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetNuke.Services.Log.EventLog;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public static class Logging
    {
        public static void LogToDnn(string key, string message, LogInfo logInfo = null, DnnHelper dnn = null)
        {
            try
            {
                if (logInfo == null)
                    logInfo = new LogInfo();

                if (dnn != null)
                {
                    logInfo.LogUserName = dnn.User?.DisplayName ?? "unknown";
                    logInfo.LogUserID = dnn.User?.UserID ?? -1;
                    logInfo.LogPortalID = dnn.Portal.PortalId;
                    logInfo.AddProperty("ModuleId", dnn?.Module?.ModuleID.ToString() ?? "unknown");
                }
                if(string.IsNullOrEmpty(logInfo.LogTypeKey ))
                    logInfo.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString();

                var messages = Split(message, 480).ToList();
                var num = 1;
                messages.ForEach(m => logInfo.AddProperty("M" + num++, m));

                new EventLogController().AddLog(logInfo);
            }
            catch
            {
            }
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            var lines = str.Split('\n').ToList();
            var res = new List<string>();
            var lastLine = "";
            lines.ForEach(l =>
            {
                if (lastLine.Length + l.Length <= chunkSize - 2)
                    lastLine += l + "\n";
                else
                {
                    res.Add(lastLine);
                    lastLine = "";
                }
            });
            if(lastLine != "")
                res.Add(lastLine);
            return res;
        }

    }
}