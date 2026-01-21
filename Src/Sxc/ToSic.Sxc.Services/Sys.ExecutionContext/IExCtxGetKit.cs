using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxGetKit
{
    TKit GetKit<TKit>() where TKit : ServiceKit;
}