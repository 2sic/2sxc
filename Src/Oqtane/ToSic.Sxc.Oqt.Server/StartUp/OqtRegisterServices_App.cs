using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Security;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Apps;
using ToSic.Sxc.Oqt.Server.LookUps;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    internal static partial class OqtRegisterServices
    {


        private static IServiceCollection AddOqtaneLookUpsAndSources(this IServiceCollection services)
        {
            services.TryAddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.TryAddTransient<QueryStringLookUp>();
            services.TryAddTransient<SiteLookUp>();
            services.TryAddTransient<OqtPageLookUp>();
            services.TryAddTransient<OqtModuleLookUp>();
            services.TryAddScoped<UserLookUp>();
            return services;
        }
        

        private static IServiceCollection AddSxcOqtAdam(this IServiceCollection services)
        {
            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.TryAddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();
            return services;
        }

        private static IServiceCollection AddSxcOqtAppPermissionsAndImportExport(this IServiceCollection services)
        {
            // Permissions
            services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.TryAddTransient<IEnvironmentPermission, OqtEnvironmentPermission>();

            // Import / Export
            services.TryAddTransient<XmlExporter, OqtXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();

            return services;
        }
        

    }
}
