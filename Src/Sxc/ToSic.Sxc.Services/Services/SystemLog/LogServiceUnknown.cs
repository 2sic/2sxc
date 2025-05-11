using ToSic.Eav.Internal.Unknown;

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