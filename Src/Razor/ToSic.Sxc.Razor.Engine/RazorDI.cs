using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Code;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.Engine.DbgWip;

namespace ToSic.Sxc.Razor.Engine
{
    // ReSharper disable once InconsistentNaming
    public static class RazorDI
    {
        public static IServiceCollection AddSxcRazor(this IServiceCollection services)
        {
            services.AddTransient<DynamicCodeRoot, DynamicCodeRoot>();
            services.AddTransient<IRenderRazor, RenderRazor>();
            services.AddTransient<IEngineFinder, RazorEngineFinder>();

            // debugging
            services.AddTransient<RazorReferenceManager>();

            return services;
        }
    }
}
