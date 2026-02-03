using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Code.Sys.HotBuild;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcCodeHotBuild
{
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

        // v20
        services.TryAddTransient<ExtensionCompileReferenceService>();

        return services;
    }
}
