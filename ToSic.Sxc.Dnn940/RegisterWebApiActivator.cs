using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;

namespace ToSic.Sxc.Dnn940
{
    // ReSharper disable once UnusedMember.Global
    public class RegisterWebApiActivator : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // override the DNN 9.4 activator to activate it both the DNN way and the 2sxc way
            var dnnVersion = typeof(DotNetNuke.Startup).GetTypeInfo().Assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>();
            // var dnnVersion = FileVersionInfo.GetVersionInfo("DotNetNuke.Dll").FileVersion;
            if (new Version(dnnVersion.Version).CompareTo(new Version(9, 4)) < 0) return;

            var config = GlobalConfiguration.Configuration;

            // only override the existing one, if a special one was registered
            if (config.Services.GetService(typeof(IHttpControllerActivator)) is IHttpControllerActivator dnnActivator)
                GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                    new WebApiHttpControllerActivator {PreviousActivator = dnnActivator});
        }
    }
}
