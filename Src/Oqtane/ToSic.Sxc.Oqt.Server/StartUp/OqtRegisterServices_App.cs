using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Environment.Sys.Permissions;
using ToSic.Eav.ImportExport.Integration;

using ToSic.Eav.ImportExport.Sys.XmlExport;
using ToSic.Sxc.Adam.Sys.FileSystem;
using ToSic.Sxc.Adam.Sys.Paths;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Oqt.Server.StartUp;

internal static partial class OqtRegisterServices
{

    private static IServiceCollection AddOqtaneLookUpsAndSources(this IServiceCollection services)
    {
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
        services.TryAddTransient<IEnvironmentPermission, OqtEnvironmentPermission>();

        // Import / Export
        services.TryAddTransient<IXmlExporterIntegration, OqtXmlExporterIntegration>(); // new v22
        services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();

        return services;
    }
        

}