using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Code.CodeApi.Internal;

public static class ExecutionContextExtensions
{
    public static ICodeTypedApiService GetTypedApi(this ICodeApiService exCtx)
    {
        if (exCtx is null)
            throw new InvalidOperationException("ExecutionContext must be of type ICodeApiService");
        return exCtx.TypedApi;
    }

    public static ICodeDynamicApiService GetDynamicApi(this ICodeApiService exCtx)
    {
        if (exCtx is null)
            throw new InvalidOperationException("ExecutionContext must be of type ICodeApiService");
        return exCtx.DynamicApi;
    }
}