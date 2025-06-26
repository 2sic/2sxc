using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxAttachApp
{
    void AttachApp(IApp app);
}