using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class RegisterSxcServices
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCore(this IServiceCollection services)
    {

        return services;
    }

}