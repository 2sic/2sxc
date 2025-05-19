using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Services;

namespace ToSic.Sxc;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcAdamStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcAdam(this IServiceCollection services)
    {
        // Adam stuff
        services.TryAddTransient<IAdamSecurityCheckService, AdamSecurityChecksBasic>();
        services.TryAddTransient<AdamSecurityChecksBase.MyServices>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.TryAddTransient(typeof(AdamManager<,>));
        services.TryAddTransient(typeof(AdamContext<,>));
        services.TryAddTransient<AdamContext.MyServices>();

        services.AddTransient<AdamManager.MyServices>();

        // WIP v14
        services.TryAddTransient<IAdamService, AdamService>();

        //services.AddSxcAdamWork();

        //// Add possibly missing fallback services
        //// This must always be at the end here so it doesn't accidentally replace something we actually need
        services.AddSxcAdamFallbackServices();

        return services;
    }

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcAdamWork<TFolder, TFile>(this IServiceCollection services)
    {
        // Helper Services
        services.TryAddTransient(typeof(AdamWorkBase<,>.MyServices));

        // Generic Services, untyped; used when other services request helpers
        services.TryAddTransient(typeof(AdamWorkGet<,>));
        services.TryAddTransient(typeof(AdamWorkFolderCreate<,>));
        services.TryAddTransient(typeof(AdamWorkDelete<,>));
        services.TryAddTransient(typeof(AdamWorkUpload<,>));
        services.TryAddTransient(typeof(AdamWorkRename<,>));

        // Storage
        services.TryAddTransient(typeof(AdamStorageOfSite<,>));
        services.TryAddTransient(typeof(AdamStorageOfField<,>));

        // Typed implementations, as specified by the caller; usually `int`
        services.TryAddTransient<IAdamWorkGet, AdamWorkGet<TFolder, TFile>>();

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
    public static IServiceCollection AddSxcAdamFallbackServices(this IServiceCollection services)
    {
        // ADAM basics
        // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        services.TryAddTransient<IAdamFileSystem<string, string>, AdamFileSystemBasic>();

        return services;
    }

}