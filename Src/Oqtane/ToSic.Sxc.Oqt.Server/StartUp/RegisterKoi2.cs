using Connect.Koi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Oqt.Server.Polymorphism;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    public static class RegisterKoi2
    {

        public static IServiceCollection ActivateKoi2Di(this IServiceCollection services)
        {
            services.TryAddTransient<KoiCss.Dependencies>();
            services.TryAddTransient<ICss, KoiCss>();
            services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, OqtKoiCssFrameworkDetector>();
            return services;
        }


    }
}