using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcRenderStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcRender(this IServiceCollection services)
    {
        services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
        services.TryAddTransient<RenderService.MyServices>();
        services.TryAddTransient<SimpleRenderer>();
        services.TryAddTransient<InTextContentBlockRenderer>();
#if NETFRAMEWORK
#pragma warning disable CS0618
        services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618
#endif
        services.TryAddTransient<BlockBuilder>();
        services.TryAddTransient<BlockBuilder.MyServices>();

        services.TryAddTransient<IBlockBuilder, BlockBuilder>();
        services.TryAddTransient<IRenderingHelper, RenderingHelper>();



        return services;
    }


        
}