using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.DotNet;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.Engine.DbgWip;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Razor
{
    // ReSharper disable once InconsistentNaming
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class StartupRazor
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static IServiceCollection AddSxcRazor(this IServiceCollection services)
        {
            // .net Core parts
            services.TryAddTransient<IHttp, HttpNetCore>();

            // Razor Parts
            services.TryAddTransient<IRazorCompiler, RazorCompiler>();
            services.TryAddTransient<IRazorRenderer, RazorRenderer>();
            services.TryAddTransient<IRazorEngine, NetCoreRazorEngine>();
            
            // debugging
            services.TryAddTransient<RazorReferenceManager>();

            // Web
            services.TryAddTransient<IRazorService, RazorService>();

            return services;
        }
    }
}
