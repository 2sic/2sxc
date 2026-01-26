using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Sxc.Adam.Sys.FileSystem;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Adam.Sys.Paths;
using ToSic.Sxc.Adam.Sys.Security;
using ToSic.Sxc.Adam.Sys.Storage;
using ToSic.Sxc.Adam.Sys.Work;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcAdam
{

    public static IServiceCollection AddSxcAdam(this IServiceCollection services)
    {
        // Adam stuff
        services.TryAddTransient<IAdamSecurityCheckService, AdamSecurityChecksBasic>();
        services.TryAddTransient<AdamSecurityChecksBase.Dependencies>();
        services.TryAddTransient<IAdamPaths, AdamPathsBase>();
        services.TryAddTransient<AdamConfiguration>();

        services.TryAddTransient<AdamManager>();
        services.TryAddTransient<AdamContext>();
        services.TryAddTransient<AdamContext.Dependencies>();

        services.AddTransient<AdamManager.Dependencies>();

        // WIP v14
        services.TryAddTransient<IAdamService, AdamService>();

        //services.AddSxcAdamWork();

        //// Add possibly missing fallback services
        //// This must always be at the end here so it doesn't accidentally replace something we actually need
        services.AddSxcAdamFallbacks();

        return services;
    }

    public static IServiceCollection AddSxcAdamWork<TFolder, TFile>(this IServiceCollection services)
    {
        // Helper Services
        services.TryAddTransient<AdamWorkBase.Dependencies>();

        // Generic Services, untyped; used when other services request helpers
        services.TryAddTransient<AdamWorkGet>();
        services.TryAddTransient<AdamWorkFolderCreate>();
        services.TryAddTransient<AdamWorkDelete>();
        services.TryAddTransient<AdamWorkUpload>();
        services.TryAddTransient<AdamWorkRename>();

        // Storage
        services.TryAddTransient<AdamStorageOfSite>();
        services.TryAddTransient<AdamStorageOfField>();

        // Typed implementations, as specified by the caller; usually `int`
        services.TryAddTransient<AdamGenericHelper, AdamGenericHelper<TFolder, TFile>>();

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
    public static IServiceCollection AddSxcAdamFallbacks(this IServiceCollection services)
    {
        // ADAM basics
        // TODO: this doesn't warn yet, there should be an AdamFileSystemUnknown(WarnUseOfUnknown<AdamFileSystemUnknown> warn)
        services.TryAddTransient<IAdamFileSystem, AdamFileSystemString>();

        return services;
    }

}