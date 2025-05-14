using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class RegisterSxcServices
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCore(this IServiceCollection services)
    {
        // Adam stuff
        services.TryAddTransient<AdamSecurityChecksBase, AdamSecurityChecksBasic>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.AddTransient<AdamManager.MyServices>();

        
        //// Add possibly missing fallback services
        //// This must always be at the end here so it doesn't accidentally replace something we actually need
        //services.AddKoi();
        services.AddSxcCoreFallbackServices();

        return services;
    }

    /// <summary>
    /// This will add Do-Nothing services which will take over if they are not provided by the main system
    /// In general this will result in some features missing, which many platforms don't need or care about
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <remarks>
    /// All calls in here MUST use TryAddTransient, and never without the Try
    /// </remarks>
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcCoreFallbackServices(this IServiceCollection services)
    {
        // ADAM basics
        // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        return services;
    }
}