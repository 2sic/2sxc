using DotNetNuke.Web.Api;
using System.Configuration;
using System.Web.Hosting;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Loaders;
using ToSic.Eav.StartUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Dnn.StartUp;

/// <summary>
/// This configures .net Core Dependency Injection
/// The StartUp is defined as an IServiceRouteMapper.
/// This way DNN will auto-run this code before anything else
/// </summary>
// ReSharper disable once UnusedMember.Global
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class StartupDnn : IServiceRouteMapper
{
    /// <summary>
    /// This will be called by DNN when loading the assemblies.
    /// We just want to trigger the DI-Configure
    /// </summary>
    /// <param name="mapRouteManager"></param>
    public void RegisterRoutes(IMapRoute mapRouteManager) => Configure();


    private static bool _alreadyConfigured;

    /// <summary>
    /// Configure IoC for 2sxc. If it's already configured, do nothing.
    /// </summary>
    public bool Configure()
    {
        var l = BootLog.Log.Fn<bool>("Dnn: Configuring WebApi Routes", timer: true);

        // In some cases this may be called 2x - so we must avoid doing it again
        if (_alreadyConfigured)
            return l.ReturnFalse();

        // Configure Newtonsoft Time zone handling etc. - part of WebApi
        StartUpDnnWebApi.Configure();

        // Getting the service provider in Configure is tricky business, because
        // of .net core 2.1 bugs
        // ATM it appears that the service provider will get destroyed after startup, so we MUST get an additional one to use here
        // 2023-06-15 2dm - making sure that even if we use the global DI, we're always using it in a scope to never bleed global objects
        var transientSp = DnnStaticDi.GetGlobalScopedServiceProvider();

        // now we should be able to instantiate registration of DB
        transientSp.Build<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
        var globalConfig = transientSp.Build<IGlobalConfiguration>();

        globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);
        globalConfig.AssetsVirtualUrl = DnnConstants.SysFolderRootVirtual + "assets/";
        globalConfig.SharedAppsFolder = "~/Portals/_default/" + AppConstants.AppsRootFolder + "/";
        globalConfig.TempAssemblyFolder = HostingEnvironment.MapPath($"~/{Eav.Constants.AppDataProtectedFolder}/{Eav.Constants.TempAssemblyFolder}/"); // ".../App_Data/2sxc.bin"
        globalConfig.CryptoFolder = HostingEnvironment.MapPath($"~/{Eav.Constants.AppDataProtectedFolder}/{Eav.Constants.CryptoFolder}/");

        var sxcSysLoader = transientSp.Build<SystemLoader>();
        sxcSysLoader.StartUp();

        // After the SysLoader got the features, we must attach it to an old API which had was public
        // This was used in Mobius etc. to see if features are activated
#pragma warning disable CS0618
        Eav.Configuration.Features.FeaturesFromDi = sxcSysLoader.EavSystemLoader.Features;
#pragma warning restore CS0618

        // Optional registration of query string rewrite functionality implementation for dnn imageflow module
        Imageflow.Dnn.StartUp.RegisterQueryStringRewrite(ImageflowRewrite.QueryStringRewrite);

        // Clean the App_Data/2sxc.bin folder
        transientSp.Build<Util>().CleanTempAssemblyFolder();

        _alreadyConfigured = true;
        return l.ReturnTrue();
    }
}