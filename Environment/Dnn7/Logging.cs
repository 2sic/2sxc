using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Services.Log.EventLog;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public static class Logging
    {
        public static void LogToDnn(string key, string message, Log log = null, DnnHelper dnn = null)
        {
            try
            {
                //if (logInfo == null)
                    var logInfo = new LogInfo();

                if (dnn != null)
                {
                    logInfo.LogUserName = dnn.User?.DisplayName ?? "unknown";
                    logInfo.LogUserID = dnn.User?.UserID ?? -1;
                    logInfo.LogPortalID = dnn.Portal.PortalId;
                    logInfo.AddProperty("ModuleId", dnn.Module?.ModuleID.ToString() ?? "unknown");
                }
                if(string.IsNullOrEmpty(logInfo.LogTypeKey ))
                    logInfo.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString();


                if(!string.IsNullOrEmpty(message))
                    logInfo.AddProperty(key, message);

                log?.Entries.ForEach(e => logInfo.AddProperty(e.Source, e.Message));
                //var messages = Split(message, 480).ToList();
                //var num = 1;
                //messages = message.Split('\n').ToList();
                //messages.ForEach(m => logInfo.AddProperty("M" + num++, "\n" + m));
            
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