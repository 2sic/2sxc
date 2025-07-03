using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Edit.EditService;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcEditStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
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