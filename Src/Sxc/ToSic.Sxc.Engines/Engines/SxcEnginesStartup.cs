using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcEnginesStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcEngines(this IServiceCollection services)
    {
        services.TryAddTransient<TokenEngine>();

        services.TryAddTransient<EngineFactory>();
        services.TryAddTransient<IEngineFactory, EngineFactory>();

        services.TryAddTransient<EngineBase.MyServices>();
        services.TryAddTransient<EngineCheckTemplate>();
        services.TryAddTransient<EnginePolymorphism>();
        services.TryAddTransient<EngineAppRequirements>();

        return services;
    }


        
}