using ToSic.Lib.Services;

namespace ToSic.Sxc.Services;

[PrivateApi("Mock / Unknown implementation")]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class SystemLogServiceUnknown(WarnUseOfUnknown<SystemLogServiceUnknown> _) : ISystemLogService
{
    public void Add(string title, string message)
    {
        // ignore
    }
}