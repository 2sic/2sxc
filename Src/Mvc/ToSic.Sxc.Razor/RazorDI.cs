using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Code;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.Code;
using ToSic.Sxc.Razor.Engines;

namespace ToSic.Sxc.Razor
{
    public static class RazorDI
    {
        public static IServiceCollection AddSxcRazor(this IServiceCollection services)
        {
            services.AddTransient<DynamicCodeRoot, Razor3DynamicCode>();
            services.AddTransient<IRenderRazor, RenderRazor>();
            services.AddTransient<IEngineFinder, RazorEngineFinder>();

            return services;
        }
    }
}
