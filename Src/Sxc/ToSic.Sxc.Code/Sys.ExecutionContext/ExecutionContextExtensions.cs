using ToSic.Eav.DataSource;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Sys.ExecutionContext;

internal static class ExecutionContextExtensions
{
    public static ContextData GetContextData(this IExecutionContext exCtx)
    {
        return (ContextData)exCtx.GetState<IDataSource>();
    }
}
