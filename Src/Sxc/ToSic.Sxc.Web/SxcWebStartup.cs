using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Web.Internal.EditUi;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcWebStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcWeb(this IServiceCollection services)
    {
        // v15 EditUi Resources
        services.TryAddTransient<EditUiResources>();


        return services;
    }


        
}