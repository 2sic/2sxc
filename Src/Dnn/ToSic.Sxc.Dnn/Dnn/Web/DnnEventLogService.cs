using DotNetNuke.Services.Log.EventLog;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnEventLogService : IEventLogService
    {
        public void AddEvent(string title, string message)
        {
            var logInfo = new LogInfo
            {
                LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString()
            };
            logInfo.AddProperty(title, message);
            EventLogController.Instance.AddLog(logInfo);
        }
    }
}
