using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    public class EventLogServiceUnknown : IEventLogService
    {
        public EventLogServiceUnknown(WarnUseOfUnknown<EventLogServiceUnknown> warn)
        {
            
        }
        
        public void AddEvent(string title, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}