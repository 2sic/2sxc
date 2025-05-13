using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.Integration.Modules;
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

        services.TryAddTransient<BlockEditorSelector>();

        // Block Editors
        services.TryAddTransient<BlockEditorForEntity>();
        services.TryAddTransient<BlockEditorForModule>();
        services.TryAddTransient<BlockEditorBase.MyServices>();

        // Block functionality
        services.TryAddTransient<BlockDataSourceFactory>();
        services.TryAddTransient<DataSources.CmsBlock.MyServices>(); // new v15

        services.TryAddTransient<IAppDataConfigProvider, SxcAppDataConfigProvider>(); // new v17

        // Context stuff in general
        services.TryAddTransient<IContextOfBlock, ContextOfBlock>();
        

        // Context stuff, which is explicitly scoped
        services.TryAddScoped<ISxcContextResolver, SxcContextResolver>();
        // must be the same instance, so it must get the original, scoped SxcContextResolver
        services.TryAddTransient<ISxcAppContextResolver>(sp => sp.GetRequiredService<SxcContextResolver>());


        // New v15.04 WIP
        services.TryAddScoped<IContextResolver>(x => x.GetRequiredService<ISxcContextResolver>());
        services.TryAddScoped<IContextResolverUserPermissions>(x => x.GetRequiredService<ISxcContextResolver>());
        services.TryAddScoped<AppIdResolver>();

        // Integration stuff - must be implemented by each platform
        services.TryAddTransient<IPlatformModuleUpdater, BasicModuleUpdater>();


        services.AddSxcBlocksFallback();

        return services;
    }

    public static IServiceCollection AddSxcBlocksFallback(this IServiceCollection services)
    {

        services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

        return services;
    }


        
}