using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    internal static partial class OqtRegisterServices
    {

        /// <summary>
        /// Context information like the ISite, IUser etc.
        /// </summary>
        private static IServiceCollection AddSxcOqtContext(this IServiceCollection services)
        {
            // Context: Things which are relevant for determining the context
            services.TryAddScoped<ISite, OqtSite>();
            services.TryAddScoped<IPage, OqtPage>();
            // TODO: @STV - this could cause a bug, because OqtSite and ISite are both scoped, but no the same object
            // TODO: The OqtSite should probably not be scoped, or instead use the ISite 
            services.TryAddScoped<OqtSite>();
            services.TryAddScoped<IUser, OqtUser>();
            services.TryAddTransient<IModule, OqtModule>();
            // 2022-02-21 2dm disabled, don't think this is used in DI
            // services.TryAddTransient<OqtModule>();

            // TODO: @STV - this could cause a bug - maybe better not to mix this? OqtSite transient?
            services.TryAddTransient<IZoneCultureResolver, OqtSite>();

            return services;
        }
    }
}
