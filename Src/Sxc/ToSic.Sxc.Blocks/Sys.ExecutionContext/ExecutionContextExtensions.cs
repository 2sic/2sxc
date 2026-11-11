using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    public static IBlock GetBlock(this IExecutionContext exCtx)
        => (exCtx as IExecutionContextWithBlock)?.Block!;

    public static IContextOfModule GetContextOfBlock(this IExecutionContext exCtx)
        => (exCtx as IExecutionContextWithBlock)?.Block?.Context!;
    public static IContextOfModule? TryGetContextOfBlock(this IExecutionContext exCtx)
        => (exCtx as IExecutionContextWithBlock)?.Block?.Context;
}