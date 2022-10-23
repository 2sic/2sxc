using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Mock / Unknown implementation")]
    public class LogServiceUnknown : ILogService
    {
        public LogServiceUnknown(WarnUseOfUnknown<LogServiceUnknown> warn)
        {
            
        }
        
        public void Add(string title, string message)
        {
            // ignore
        }
    }
}