using ToSic.Sxc.Code.Context.Sys;

namespace ToSic.Sxc.Code.Sys.CodeApi;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class CodeContextExtensions
{
    public static ICodeTypedApiHelper GetTypedApi(this ICodeContext exCtx) =>
        exCtx is not CodeContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.TypedApi;

    public static ICodeDynamicApiHelper GetDynamicApi(this ICodeContext exCtx) =>
        exCtx is not CodeContext exCtxReal
            ? throw ExCtxWrongType()
            : exCtxReal.DynamicApi;

    private static InvalidOperationException ExCtxWrongType() => new($"CodeContext must be of type {nameof(CodeContext)}");
}