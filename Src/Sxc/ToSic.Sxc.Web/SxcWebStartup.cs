using Connect.Koi.Detectors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Polymorphism.Internal;
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

        // The top version should be deprecated soon, so we just use DataToDictionary or an Interface instead
        services.TryAddTransient<ConvertToEavLight, ConvertToEavLightWithCmsInfo>(); // this is needed for all the EAV uses of conversion
        services.TryAddTransient<ConvertToEavLightWithCmsInfo>(); // WIP, not public, should use interface instead
        services.TryAddTransient<IConvertToEavLight, ConvertToEavLightWithCmsInfo>();

        // Polymorphism - moved here v17.08
        services.AddTransient<IPolymorphismResolver, PolymorphismKoi>();
        services.AddTransient<IPolymorphismResolver, PolymorphismPermissions>();

        // Koi, mainly so tests don't fail
        services.TryAddTransient<ICssFrameworkDetector, CssFrameworkDetectorUnknown>();


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