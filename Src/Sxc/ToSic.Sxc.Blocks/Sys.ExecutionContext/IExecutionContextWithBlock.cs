using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextWithBlock
{
    public IBlock? Block { get; }
    ContextData BlockData { get; }
}