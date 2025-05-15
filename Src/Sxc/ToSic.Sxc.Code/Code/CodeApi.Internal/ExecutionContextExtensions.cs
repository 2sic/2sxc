using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Code.CodeApi.Internal;

public static class ExecutionContextExtensions
{
    public static ICodeTypedApiHelper GetTypedApi(this ICodeApiService exCtx)
    {
        if (exCtx is not CodeApiService exCtxReal)
            throw new InvalidOperationException("ExecutionContext must be of type ICodeApiService");
        return exCtxReal.TypedApi;
    }

    public static ICodeDynamicApiHelper GetDynamicApi(this ICodeApiService exCtx)
    {
        if (exCtx is not CodeApiService exCtxReal)
            throw new InvalidOperationException("ExecutionContext must be of type ICodeApiService");
        return exCtxReal.DynamicApi;
    }
}