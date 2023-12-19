using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Dnn.Razor;

namespace ToSic.Sxc;

public static class StartUpDnnRazor
{
    public static IServiceCollection AddDnnRazor(this IServiceCollection services)
    {
        services.TryAddTransient<HtmlHelper>();

        services.TryAddTransient<RoslynBuildManager>();
        services.TryAddSingleton<AssemblyResolver>();

        return services;
    }

}