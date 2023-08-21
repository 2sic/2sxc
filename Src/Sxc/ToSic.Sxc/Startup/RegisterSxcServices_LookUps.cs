using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;

namespace ToSic.Sxc.Startup
{
    public static partial class RegisterSxcServices
    {

        /// <summary>
        /// This will add LookUps for DI
        /// All must use AddTransient, not TryAdd
        /// </summary>
        public static IServiceCollection AddSxcCoreLookUps(this IServiceCollection services)
        {
#if NETCOREAPP
            services.TryAddTransient<ILookUpEngineResolver, LookUpEngineResolverGeneric>();

            services.AddTransient<ILookUp, QueryStringLookUp>();
            services.AddTransient<ILookUp, DateTimeLookUp>();
            services.AddTransient<ILookUp, TicksLookUp>();
#else
            services.TryAddTransient<QueryStringLookUp>();
#endif

            return services;
        }
    }
}