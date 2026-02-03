using ToSic.Sxc.Context;

namespace ToSic.Sxc.Sys.ExecutionContext;

public static class ExecutionContextExtensions
{
    public static ICmsContext GetCmsContext(this IExecutionContext context)
        => context.GetState<ICmsContext>();
}
