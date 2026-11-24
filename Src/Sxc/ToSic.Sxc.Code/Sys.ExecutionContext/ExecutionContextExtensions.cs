using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Sys.ExecutionContext;

internal static class ExecutionContextExtensions
{
    public static ContextData GetContextData(this ICodeContext exCtx)
        => (ContextData?)((ExecutionContext)exCtx).Block?.Data
           ?? throw new NullReferenceException($"{nameof(ExecutionContext.Block)} isn't available");
}
