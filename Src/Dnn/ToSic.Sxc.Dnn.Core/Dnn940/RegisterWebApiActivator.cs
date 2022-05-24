using DotNetNuke.Web.Api;
using System.Web.Http;
using System.Web.Http.Dispatcher;

// This is a special workaround for DNN 9.4
// Review the readme.md to understand how and why

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn940
{
    // ReSharper disable once UnusedMember.Global
    public class RegisterWebApiActivator : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // commented out since 2022-05-25 because in 2sxc v14 minimal version of dnn is 9.6.1
            //var dnnVersion = DotNetNukeContext.Current.Application.Version;

            //if (dnnVersion.CompareTo(new Version(9, 4)) < 0) return;

            var config = GlobalConfiguration.Configuration;

            // only override the existing one, if a special one was registered
            if (config.Services.GetService(typeof(IHttpControllerActivator)) is IHttpControllerActivator dnnActivator)
                GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                    new WebApiHttpControllerActivator {PreviousActivator = dnnActivator});
        }
    }
}
