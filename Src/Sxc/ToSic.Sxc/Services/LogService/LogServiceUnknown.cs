using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Mock / Unknown implementation")]
    internal class SystemLogServiceUnknown : ISystemLogService
    {
        public SystemLogServiceUnknown(WarnUseOfUnknown<SystemLogServiceUnknown> _)
        {
            
        }
        
        public void Add(string title, string message)
        {
            // ignore
        }
    }
}