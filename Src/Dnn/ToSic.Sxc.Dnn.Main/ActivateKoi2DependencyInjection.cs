using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ToSic.SexyContent
{
    public static class ActivateKoi2DependencyInjection
    {

        public static IServiceCollection ActivateKoi2Di(this IServiceCollection services)
        {
            services.TryAddTransient<Connect.Koi.KoiCss.Dependencies>();
            services.TryAddTransient<Connect.Koi.ICss, Connect.Koi.KoiCss>();
            services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();
            return services;
        }


    }
}