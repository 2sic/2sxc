using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Connect.Koi; 

namespace ToSic.Sxc.Oqt.Server.Extensions.Koi
{
    public static class RegisterKoi2
    {

        public static IServiceCollection ActivateKoi2Di(this IServiceCollection services)
        {
            services.TryAddTransient<KoiCss.Dependencies>();
            services.TryAddTransient<ICssInfo, KoiCss>();
            services.TryAddTransient<ICssBuilder, KoiCss>();
            services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, OqtKoiCssFrameworkDetector>();
            return services;
        }


    }
}