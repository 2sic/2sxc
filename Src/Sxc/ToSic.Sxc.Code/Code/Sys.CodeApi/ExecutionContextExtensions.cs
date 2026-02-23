using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApi;

public static class ExecutionContextExtensions
{
    public static ICodeTypedApiHelper GetTypedApi(this IExecutionContext exCtx) =>
        exCtx is not ExecutionContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.TypedApi;

    public static ICodeDynamicApiHelper GetDynamicApi(this IExecutionContext exCtx) =>
        exCtx is not ExecutionContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.DynamicApi;

    public static int GetAppId(this IExecutionContext exCtx) =>
        exCtx is not ExecutionContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.App.AppId;

    private static InvalidOperationException ExCtxWrongType() => new($"ExecutionContext must be of type {nameof(ExecutionContext)}");
}