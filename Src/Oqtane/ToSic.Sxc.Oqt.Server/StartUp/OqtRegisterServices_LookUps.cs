using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.LookUps;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{
    /// <summary>
    /// Mail, Logging and other services.
    /// </summary>
    private static IServiceCollection AddSxcOqtLookUps(this IServiceCollection services)
    {
        services.TryAddTransient<ILookUp, OqtModuleLookUp>();
        services.TryAddTransient<ILookUp, OqtPageLookUp>();
        services.TryAddTransient<ILookUp, OqtSiteLookUp>();
        services.TryAddTransient<ILookUp, OqtUserLookUp>();
        return services;
    }
}