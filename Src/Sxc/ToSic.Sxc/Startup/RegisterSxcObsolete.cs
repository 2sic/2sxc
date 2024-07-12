using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Web;

#pragma warning disable CS0618
#pragma warning disable CS0612

namespace ToSic.Sxc.Startup;

internal static class RegisterSxcObsolete
{
    public static IServiceCollection AddObsoleteServicesAndKits(this IServiceCollection services)
    {
            services.TryAddTransient<Web.IPageService, WebPageServiceObsolete>();  // Obsolete version, needed to keep old Apps working which used this
            return services;
        }
}