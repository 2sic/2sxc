using DotNetNuke.Services.Log.EventLog;
using ToSic.Sxc.Services;
using DotNetNuke.Abstractions.Logging;

namespace ToSic.Sxc.Dnn.Services;

internal class DnnSystemLogService(IEventLogger eventLogger) : ISystemLogService,
    ILogService // for compatibility
{
    public void Add(string title, string message)
    {
        var logInfo = new LogInfo
        {
            LogTypeKey = EventLogType.ADMIN_ALERT.ToString()
        };
        logInfo.AddProperty(title, message);
        eventLogger.AddLog(logInfo);
    }
}
