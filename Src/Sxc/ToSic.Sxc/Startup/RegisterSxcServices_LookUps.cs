using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.LookUp.Internal;

namespace ToSic.Sxc.Startup;

static partial class RegisterSxcServices
{

    /// <summary>
    /// This will add LookUps for DI
    /// All must use AddTransient, not TryAdd
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddSxcCoreLookUps(this IServiceCollection services)
    {
        // QueryStringLookUp is used in Dnn and Oqtane
        // It does both standard QueryString lookup but also respects OriginalParameters for AJAX cases

        services.AddTransient<ILookUp, QueryStringLookUp>();

        // This is more of a fallback, in DNN it's pre-registered so it won't use this
        services.TryAddTransient<ILookUpEngineResolver, LookUpEngineResolver>();

#if NETCOREAPP
        services.AddTransient<ILookUp, DateTimeLookUp>();
        services.AddTransient<ILookUp, TicksLookUp>();
#endif

        return services;
    }
}