using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtEventLogService : IEventLogService
    {
        private readonly ILogManager _logManager;

        public OqtEventLogService(ILogManager logManager)
        {
            _logManager = logManager;
        }
        
        public void AddEvent(string title, string message)
        {
            _logManager.Log(LogLevel.Information, title, LogFunction.Other, message);
        }
    }
}
