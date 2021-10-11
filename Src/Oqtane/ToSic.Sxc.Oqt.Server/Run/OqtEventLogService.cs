using System.Reflection;
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

    // TODO: WIP, need to enhance Logging in Oqtane using LogManager, but find better method (like prefill new Log())
    // because "Title" is not stored log as expected 

    //public class SxcAppEventLogHost
    //{
    //    public SxcAppEventLogHost(string title)
    //    {
    //        Title = title;
    //    }

    //    public string Title { get; }

    //    public override string? ToString()
    //    {
    //        return Title;
    //    }
    //}
}
