using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class IExecutionContextCdf
{
    public static ICodeDataFactory GetCdf(this IExecutionContext exCtx)
        => ((IWrapper<ICodeDataFactory>)exCtx).GetContents()!;
}
