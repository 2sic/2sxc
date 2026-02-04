using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Data.Sys.Wrappers;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcData
{
    public static IServiceCollection AddSxcData(this IServiceCollection services)
    {

        // v16 DynamicJacket and DynamicRead factories
        services.TryAddTransient<ICodeDataPoCoWrapperService, CodeDataPoCoWrapperService>();
        services.TryAddTransient<CodeJsonWrapper>();
        services.TryAddTransient<WrapObjectTyped>();
        services.TryAddTransient<WrapObjectTypedItem>();

        return services;
    }


        
}