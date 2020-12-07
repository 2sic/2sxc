using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Mvc
{
    /// <summary>
    /// Register types which are necessary for the system to boot, but are not used in a basic
    /// headless setup
    /// </summary>
    public static class StartupNotImplemented
    {
        public static IServiceCollection AddNotImplemented(this IServiceCollection services)
        {
            return services;
        }
    }
}
