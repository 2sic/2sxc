using DotNetNuke.Services.Log.EventLog;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Web
{
    public class DnnLogService : ILogService
    {
        public void Add(string title, string message)
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
