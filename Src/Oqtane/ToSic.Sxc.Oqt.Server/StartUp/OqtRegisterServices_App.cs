﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.Internal.Environment;
using ToSic.Sxc.Adam.FileSystem.Internal;
using ToSic.Sxc.Adam.Paths.Internal;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Oqt.Server.StartUp;

internal static partial class OqtRegisterServices
{


    private static IServiceCollection AddOqtaneLookUpsAndSources(this IServiceCollection services)
    {
        // 2023-11-28 This looks faulty - they are already registered as ILookup elsewhere
        //services.TryAddTransient<OqtSiteLookUp>();
        //services.TryAddTransient<OqtPageLookUp>();
        //services.TryAddTransient<OqtModuleLookUp>();
        //services.TryAddScoped<OqtUserLookUp>();

        // New v15 for better DI/Logging
        services.TryAddTransient<OqtAssetsFileHelper>();
        return services;
    }
        

    private static IServiceCollection AddSxcOqtAdam(this IServiceCollection services)
    {
        // ADAM stuff
        services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
        services.TryAddTransient<IAdamFileSystem, OqtAdamFileSystem>();
        return services;
    }

    private static IServiceCollection AddSxcOqtAppPermissionsAndImportExport(this IServiceCollection services)
    {
        // Permissions
        //services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
        services.TryAddTransient<IEnvironmentPermission, OqtEnvironmentPermission>();

        // Import / Export
        services.TryAddTransient<XmlExporter, OqtXmlExporter>();
        services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();

        return services;
    }
        

}