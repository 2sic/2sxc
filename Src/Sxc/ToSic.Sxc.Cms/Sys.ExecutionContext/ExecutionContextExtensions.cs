using ToSic.Sxc.Context;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    public static ICmsContext GetCmsContext(this IExecutionContext context)
        => context.GetState<ICmsContext>();
}
