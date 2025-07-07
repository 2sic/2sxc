using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.DataSources.Sys;
using ToSic.Sxc.Integration.Modules;
using ToSic.Sxc.LookUp.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users.Permissions;

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
        services.TryAddTransient<BlockGeneratorHelpers>();

        services.TryAddTransient<BlockEditorSelector>();

        // Block Editors
        services.TryAddTransient<BlockEditorForEntity>();
        services.TryAddTransient<BlockEditorForModule>();
        services.TryAddTransient<BlockEditorBase.Dependencies>();

        // Block functionality
        services.TryAddTransient<BlockDataSourceFactory>();
        services.TryAddTransient<DataSources.CmsBlock.Dependencies>(); // new v15

        services.TryAddTransient<IAppDataConfigProvider, SxcAppDataConfigProvider>(); // new v17

        // Context stuff in general
        services.TryAddTransient<IContextOfBlock, ContextOfBlock>();



        // Context stuff, which is explicitly scoped
        services.TryAddTransient<IContextOfApp, ContextOfApp>();
        services.TryAddTransient<ContextOfApp.Dependencies>();
        services.TryAddScoped<ISxcCurrentContextService, SxcCurrentContextService>();
        // must be the same instance, so it must get the original, scoped SxcContextResolver
        services.TryAddTransient<ISxcAppCurrentContextService>(sp => sp.GetRequiredService<ISxcCurrentContextService>());


        // New v15.04 WIP
        services.TryAddScoped<ICurrentContextService>(x => x.GetRequiredService<ISxcCurrentContextService>());
        services.TryAddScoped<ICurrentContextUserPermissionsService>(x => x.GetRequiredService<ISxcCurrentContextService>());
        services.TryAddScoped<AppIdResolver>();

        // Integration stuff - must be implemented by each platform
        services.TryAddTransient<IPlatformModuleUpdater, BasicModuleUpdater>();

        // Work
        services.TryAddTransient<WorkBlocks>();
        services.TryAddTransient<WorkBlocksMod>();
        services.TryAddTransient<WorkBlockViewsGet>();

        services.AddSxcBlocksFallback();

        return services;
    }

    public static IServiceCollection AddSxcBlocksFallback(this IServiceCollection services)
    {

        services.TryAddTransient<IModuleAndBlockBuilder, ModuleAndBlockBuilderUnknown>();

        // This is more of a fallback, in DNN it's pre-registered so it won't use this
        services.TryAddTransient<ILookUpEngineResolver, LookUpEngineResolver>();

        return services;
    }


        
}