using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Web.Internal.DotNet;
using ToSic.Sxc.Web.Internal.EditUi;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcWebStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcWeb(this IServiceCollection services)
    {
        // v15 EditUi Resources
        services.TryAddTransient<EditUiResources>();

        services.AddTransient<ILookUp, QueryStringLookUp>();

        // Add Lookups which are in DNN but not in Oqtane
        // These could be in any project, but for now we want them as far down as possible, so they are in Sxc.Web
#if NETCOREAPP
        services.AddTransient<ILookUp, DateTimeLookUp>();
        services.AddTransient<ILookUp, TicksLookUp>();
#endif

        // WIP - add net-core specific stuff
        services.AddNetVariations();


        return services;
    }

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddNetVariations(this IServiceCollection services)
    {
#if NETFRAMEWORK
        // WebForms implementations
        services.TryAddScoped<IHttp, HttpNetFramework>();
#else
        services.TryAddTransient<IHttp, HttpNetCore>();
#endif
        return services;
    }

}