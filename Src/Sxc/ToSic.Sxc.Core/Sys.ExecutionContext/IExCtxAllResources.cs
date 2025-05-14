using ToSic.Sxc.Data;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxAllResources
{
    ITypedStack AllResources { get; }
}