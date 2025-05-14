using ToSic.Sxc.Data;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxAllSettings
{
    ITypedStack AllSettings { get; }
}