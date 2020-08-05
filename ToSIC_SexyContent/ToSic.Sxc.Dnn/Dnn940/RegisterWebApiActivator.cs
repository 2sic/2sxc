using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;

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
            // todo: 2020-01-07 probably replace the version check with this:
            // DotNetNukeContext.Current.Application.Version

            // override the DNN 9.4 activator to activate it both the DNN way and the 2sxc way
            var dnnVersion = typeof(DotNetNuke.Common.Globals).GetTypeInfo()
                .Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

            if (new Version(dnnVersion.Version).CompareTo(new Version(9, 4)) < 0) return;

            var config = GlobalConfiguration.Configuration;

            // only override the existing one, if a special one was registered
            if (config.Services.GetService(typeof(IHttpControllerActivator)) is IHttpControllerActivator dnnActivator)
                GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                    new WebApiHttpControllerActivator {PreviousActivator = dnnActivator});
        }
    }
}
