using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxBlock
{
    IBlock Block { get; }
}