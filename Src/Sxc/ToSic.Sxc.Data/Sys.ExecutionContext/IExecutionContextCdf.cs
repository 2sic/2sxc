using ToSic.Lib.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Sys.ExecutionContext;
public static class IExecutionContextCdf
{
    public static ICodeDataFactory GetCdf(this IExecutionContext exCtx)
        => ((IWrapper<ICodeDataFactory>)exCtx).GetContents();
}
