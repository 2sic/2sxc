using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextWithBlock
{
    public IBlock? Block { get; }
}