using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.LookUp.Internal;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcBlocksStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcBlocks(this IServiceCollection services)
    {
        // Note: not sure if this is the best way, it's connected to the blocks needing services
        services.TryAddTransient<BlockOfModule>();
        services.TryAddTransient<BlockOfEntity>();
        services.TryAddTransient<BlockServices>();

        services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
        services.TryAddTransient<RenderService.MyServices>();
        services.TryAddTransient<SimpleRenderer>();
        services.TryAddTransient<InTextContentBlockRenderer>();
#if NETFRAMEWORK
#pragma warning disable CS0618
        services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618
#endif

        services.TryAddTransient<BlockEditorSelector>();

        // Block Editors
        services.TryAddTransient<BlockEditorForEntity>();
        services.TryAddTransient<BlockEditorForModule>();
        services.TryAddTransient<BlockEditorBase.MyServices>();

        services.TryAddTransient<BlockBuilder>();
        services.TryAddTransient<BlockBuilder.MyServices>();

        services.TryAddTransient<IBlockBuilder, BlockBuilder>();
        services.TryAddTransient<IRenderingHelper, RenderingHelper>();

        // Block functionality
        services.TryAddTransient<BlockDataSourceFactory>();
        services.TryAddTransient<DataSources.CmsBlock.MyServices>(); // new v15

        services.TryAddTransient<IAppDataConfigProvider, SxcAppDataConfigProvider>(); // new v17


        services.AddSxcBlocksFallback();

        return services;
    }

    public static IServiceCollection AddSxcBlocksFallback(this IServiceCollection services)
    {

        services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();


        return services;
    }


        
}