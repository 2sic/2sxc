using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Dnn.Razor;

namespace ToSic.Sxc;

public static class StartUpDnnRazor
{
    public static IServiceCollection AddDnnRazor(this IServiceCollection services)
    {
        services.TryAddTransient<DnnRazorSourceAnalyzer>();
        services.TryAddTransient<HtmlHelper>();
        services.TryAddTransient<RoslynBuildManager>();

        return services;
    }

}