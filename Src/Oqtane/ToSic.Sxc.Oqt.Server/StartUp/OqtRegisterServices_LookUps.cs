using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.LookUp;
using ToSic.Lib.LookUp;
using ToSic.Sxc.Oqt.Server.LookUps;

namespace ToSic.Sxc.Oqt.Server.StartUp;

partial class OqtRegisterServices
{
    /// <summary>
    /// Mail, Logging and other services.
    /// </summary>
    private static IServiceCollection AddSxcOqtLookUps(this IServiceCollection services)
    {
        services.AddTransient<ILookUp, OqtModuleLookUp>();
        services.AddTransient<ILookUp, OqtPageLookUp>();
        services.AddTransient<ILookUp, OqtSiteLookUp>();
        services.AddTransient<ILookUp, OqtUserLookUp>();
        return services;
    }
}