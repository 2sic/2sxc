using ToSic.Sxc.Services;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxGetKit
{
    TKit GetKit<TKit>() where TKit : ServiceKit;
}