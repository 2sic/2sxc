using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    public static IBlock GetBlock(this IExecutionContext exCtx)
        => exCtx.GetState<IBlock>();

    public static IContextOfBlock GetContextOfBlock(this IExecutionContext exCtx)
        => exCtx.GetState<IContextOfBlock>();
}
