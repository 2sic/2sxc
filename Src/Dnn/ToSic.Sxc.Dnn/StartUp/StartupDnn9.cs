using System.Web;
using DotNetNuke.Common;
using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Dnn.StartUp
{
    /// <summary>
    /// This is the preferred way to start Dependency Injection, but it requires Dnn 9.4+
    /// If an older version of Dnn is used, this code will not run
    /// </summary>
    public class StartupDnn9 : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Tell the static factory (used in obsolete code) about the services we have
            //Eav.Factory.UseExistingServices(services);

            // Do standard registration of all services
            // If Dnn < 9.4 is called, this will be called again from the Route-Registration code
            DnnDi.RegisterServices(services);

            // TODO: @STV
            // Give it the Dnn 9 Global Service Provider
            // https://github.com/dnnsoftware/Dnn.Platform/blob/9f83285a15d23203cbaad72d62add864ab5b8c7f/DNN%20Platform/DotNetNuke.Web/Common/LazyServiceProvider.cs#L28
            DnnDi.GetPreparedServiceProvider = () => HttpContext.Current.GetScopeDnn9().ServiceProvider; 
        }
    }

    /// <summary>
    /// Dependency injection extensions for HttpContext.
    /// https://github.com/dnnsoftware/Dnn.Platform/blob/9f83285a15d23203cbaad72d62add864ab5b8c7f/DNN%20Platform/Library/Common/Extensions/HttpContextDependencyInjectionExtensions.cs
    /// </summary>
    public static class PartOfDnn9HttpContextDependencyInjectionExtensions
    {
        /// <summary>
        /// Gets the http context service scope.
        /// </summary>
        /// <param name="httpContext">The http context from which to get the scope from.</param>
        /// <returns>A service scope.</returns>
        public static IServiceScope GetScopeDnn9(this HttpContext httpContext)
        {
            return GetScope(httpContext.Items);
        }

        /// <summary>
        /// Gets the service scope from a collection of context items.
        /// </summary>
        /// <param name="contextItems">A dictionary of context items.</param>
        /// <returns>The found service scope.</returns>
        internal static IServiceScope GetScope(System.Collections.IDictionary contextItems)
        {
            if (!contextItems.Contains(typeof(IServiceScope)))
            {
                return null;
            }

            return contextItems[typeof(IServiceScope)] is IServiceScope scope ? scope : null;
        }
    }
}