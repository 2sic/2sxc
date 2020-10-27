using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            services.TryAddTransient<DynamicCodeRoot, DynamicCodeRoot>();
            services.TryAddTransient<IRazorCompiler, RazorCompiler>();
            services.TryAddTransient<IRazorRenderer, RazorRenderer>();
            services.TryAddTransient<IEngineFinder, RazorEngineFinder>();

            // debugging
            services.TryAddTransient<RazorReferenceManager>();

            return services;
        }
    }
}
