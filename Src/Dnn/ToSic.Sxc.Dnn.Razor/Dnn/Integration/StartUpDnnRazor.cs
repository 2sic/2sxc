using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Razor;
using ToSic.Sxc.Dnn.Razor.Internal;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Integration;

public static class StartUpDnnRazor
{
    public static IServiceCollection AddDnnRazor(this IServiceCollection services)
    {
        services.TryAddTransient<IRazorEngine, DnnRazorEngine>();
        services.TryAddTransient<DnnRazorCompiler>();

        services.TryAddTransient<HtmlHelper>();

        services.TryAddTransient<IRoslynBuildManager, RoslynBuildManager>();

        return services;
    }

}