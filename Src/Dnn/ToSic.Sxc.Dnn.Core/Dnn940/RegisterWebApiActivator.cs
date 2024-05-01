using DotNetNuke.Web.Api;
using System.Web.Http;
using System.Web.Http.Dispatcher;

// This is a special workaround for DNN 9.4
// Review the readme.md to understand how and why

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn940;

// ReSharper disable once UnusedMember.Global
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RegisterWebApiActivator : IServiceRouteMapper
{
    /// <summary>
    /// Put our class activator in front of the standard DNN activator
    /// </summary>
    /// <param name="mapRouteManager"></param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void RegisterRoutes(IMapRoute mapRouteManager)
    {
        var config = System.Web.Http.GlobalConfiguration.Configuration;

        // only override the existing one, if a special one was registered
        if (config.Services.GetService(typeof(IHttpControllerActivator)) is IHttpControllerActivator dnnActivator)
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WebApiHttpControllerActivator {PreviousActivator = dnnActivator});
    }
}