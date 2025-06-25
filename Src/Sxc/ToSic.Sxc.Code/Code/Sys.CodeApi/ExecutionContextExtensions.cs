using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApi;

public static class ExecutionContextExtensions
{
    public static ICodeTypedApiHelper GetTypedApi(this IExecutionContext exCtx)
    {
        if (exCtx is not ExecutionContext exCtxReal)
            throw new InvalidOperationException($"ExecutionContext must be of type {nameof(ExecutionContext)}");
        return exCtxReal.TypedApi;
    }

    public static ICodeDynamicApiHelper GetDynamicApi(this IExecutionContext exCtx)
    {
        if (exCtx is not ExecutionContext exCtxReal)
            throw new InvalidOperationException($"ExecutionContext must be of type {nameof(ExecutionContext)}");
        return exCtxReal.DynamicApi;
    }
}