using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcCodeHotBuildStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCodeHotBuild(this IServiceCollection services)
    {
        // V17+ HotBuild
        services.TryAddTransient<AppCodeLoader>();
        services.TryAddTransient<AssemblyCacheManager>();
        services.TryAddTransient<DependenciesLoader>();
        services.TryAddSingleton<AssemblyResolver>();

        // v18
        services.TryAddSingleton<Util>();
        services.TryAddTransient<SourceCodeHasher>();

        return services;
    }


        
}