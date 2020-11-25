using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Sxc.Mvc.NotImplemented;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc
{
    /// <summary>
    /// Register types which are necessary for the system to boot, but are not used in a basic
    /// headless setup
    /// </summary>
    public static class StartupNotImplemented
    {
        public static IServiceCollection AddNotImplemented(this IServiceCollection services)
        {
            services.AddTransient<IRenderingHelper, NotImplementedRenderingHelper>();
            services.AddTransient<IEnvironmentInstaller, NotImplementedEnvironmentInstaller>();
            services.AddTransient<IGetEngine, NotImplementedGetLookupEngine>();

            services.TryAddTransient<XmlExporter, NotImplementedXmlExporter>();
            services.TryAddTransient<IEnvironmentLogger, NotImplementedEnvironmentLogger>();
            services.AddTransient<IPlatformModuleUpdater, NotImplementedModuleUpdater>();

            services.AddTransient<IImportExportEnvironment, NotImplementedImportExportEnvironment>();

            return services;

        }
    }
}
