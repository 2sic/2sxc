using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Integration;

namespace ToSic.Sxc.Dnn.WebApi.Sys;


internal static class DnnSxcWebApiContextExtensions
{
    public record ContextSupportedSpecs(bool TryBlock, bool TryApp, int AppId = -1);

    public static void PublishContextBlockOnly(this DnnWebApiHelper apiHelper, HttpRequestMessage request, ILog log)
        => apiHelper.PublishContext(request, new(TryBlock: true, TryApp: false), log);

    public static void PublishContext(this DnnWebApiHelper apiHelper, HttpRequestMessage request, ContextSupportedSpecs specs, ILog log)
    {
        // If nothing to init, exit early
        if (!specs.TryBlock && !specs.TryApp)
            return;

        apiHelper = apiHelper ?? throw new InvalidOperationException("System helper is not initialized");

        // If we want the block, do that first
        if (specs.TryBlock)
        {
            // Try to get the block from the request, and if found, share it with the current context
            var block = apiHelper.GetServiceNoExCtx<DnnGetBlock>()
                .TryGetBlockSpecsUsingModuleAndContentBlockHeaders(request);
            if (block != null)
                apiHelper.GetServiceNoExCtx<ISxcCurrentContextService>()
                    .ShareBlockSpecs(block);
        }
        
        if (specs.TryApp)
            apiHelper.GetServiceNoExCtx<ISxcCurrentContextService>()
                .ShareAppStateOptimized(specs.AppId);
    }
}       