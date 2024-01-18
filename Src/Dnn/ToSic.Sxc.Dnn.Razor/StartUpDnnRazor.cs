using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Razor.Internal;

namespace ToSic.Sxc;

public static class StartUpDnnRazor
{
    public static IServiceCollection AddDnnRazor(this IServiceCollection services)
    {
        services.TryAddTransient<HtmlHelper>();

        services.TryAddTransient<IRoslynBuildManager, RoslynBuildManager>();

        return services;
    }

}