using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Services;

[PrivateApi("Mock / Unknown implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
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