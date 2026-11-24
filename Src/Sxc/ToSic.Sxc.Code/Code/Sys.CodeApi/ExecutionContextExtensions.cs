using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApi;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    public static ICodeTypedApiHelper GetTypedApi(this ICodeContext exCtx) =>
        exCtx is not ExecutionContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.TypedApi;

    public static ICodeDynamicApiHelper GetDynamicApi(this ICodeContext exCtx) =>
        exCtx is not ExecutionContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.DynamicApi;

    private static InvalidOperationException ExCtxWrongType() => new($"ExecutionContext must be of type {nameof(ExecutionContext)}");
}