using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxAttachApp
{
    void AttachApp(IAppForCode app);
}