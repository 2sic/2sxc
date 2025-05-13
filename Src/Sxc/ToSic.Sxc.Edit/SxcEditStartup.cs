using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Edit.EditService;
using ToSic.Sxc.Services;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcEditStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcEdit(this IServiceCollection services)
    {
        services.TryAddTransient<IEditService, EditService>();

        return services;
    }


        
}