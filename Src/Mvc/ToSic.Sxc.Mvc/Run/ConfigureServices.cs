using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.ImportExport.Persistence.File;
using ToSic.Eav.LookUp;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Basic;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Code;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Basic;

namespace ToSic.Sxc.Mvc.Run
{
    public class ConfigureServices
    {
        public static void Configure(IServiceCollection sc)
        {
            sc.AddTransient<Eav.Conversion.EntitiesToDictionary, DataToDictionary>();
            sc.AddTransient<IValueConverter, BasicValueConverter>();
            sc.AddTransient<IUser, BasicUser>();

            //sc.AddTransient<XmlExporter, DnnXmlExporter>();
            //sc.AddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            sc.AddTransient<IRuntime, Runtime>();
            sc.AddTransient<IAppEnvironment, MvcEnvironment>();
            sc.AddTransient<IEnvironment, MvcEnvironment>();

            // new for .net standard
            sc.AddTransient<ITenant, MvcTenant>();
            //sc.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            sc.AddTransient<IHttp, HttpAbstraction>();
            sc.AddTransient<IRenderingHelper, MvcRenderingHelper>();
            sc.AddTransient<IZoneMapper, MvcZoneMapper>();


            // The file-importer - temporarily itself
            sc.AddTransient<XmlImportWithFiles, XmlImportFull>();

            sc.AddTransient<IClientDependencyOptimizer, BasicClientDependencyOptimizer>();
            sc.AddTransient<AppPermissionCheck, MvcPermissionCheck>();
            sc.AddTransient<DynamicCodeRoot, MvcDynamicCode>();
            sc.AddTransient<IEnvironmentConnector, MvcEnvironmentConnector>();
            sc.AddTransient<IEnvironmentInstaller, MvcEnvironmentInstaller>();
            //sc.AddTransient<IEnvironmentFileSystem, DnnFileSystem>();
            sc.AddTransient<IGetEngine, MvcGetLookupEngine>();
            sc.AddTransient<IFingerprint, BasicFingerprint>();

            // add page publishing
            sc.AddTransient<IPagePublishing, MvcPagePublishing>();
        }
    }
}
