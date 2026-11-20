using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.WebApi.Sys;


internal static class DnnSxcWebApiContextExtensions
{
    public record ContextSupportedSpecs(bool TryBlock, bool TryApp, int AppId = -1);

    public static void PublishContextBlockOnly(this ICanGetService apiHelper, HttpRequestMessage request, ILog log)
        => apiHelper.PublishContext(request, new(TryBlock: true, TryApp: false), log);

    public static void PublishContext(this ICanGetService apiHelper, HttpRequestMessage request, ContextSupportedSpecs specs, ILog log)
    {
        // If nothing to init, exit early
        if (!specs.TryBlock && !specs.TryApp)
            return;

        apiHelper = apiHelper ?? throw new InvalidOperationException("System helper is not initialized");

        // If we want the block, do that first
        if (specs.TryBlock)
        {
            var block = apiHelper.GetBlockFromRequest(request);
            if (block != null)
                apiHelper.GetService<ISxcCurrentContextService>().ShareBlockSpecs(block);
        }
        
        if (specs.TryApp)
            apiHelper.GetService<ISxcCurrentContextService>().ShareAppStateOptimized(specs.AppId);
    }
}       