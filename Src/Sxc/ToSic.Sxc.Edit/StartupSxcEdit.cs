using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Edit.EditService;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Sys;
using ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcEdit
{
    public static IServiceCollection AddSxcEdit(this IServiceCollection services)
    {
        services.TryAddTransient<IEditService, EditService>();

        // v14 Toolbar Builder
        services.TryAddTransient<IToolbarBuilder, ToolbarBuilder>();
        services.TryAddTransient<ToolbarBuilder.Dependencies>();
        services.TryAddTransient<ToolbarButtonDecoratorHelper>();

        return services;
    }
        
}