using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExConBlock
{
    [PrivateApi("WIP")]
    IBlock _Block { get; }
}