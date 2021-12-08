using System;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is a temporary helper for Dnn 7+ to help with dependency injection which is
    /// patched unto Dnn.
    /// </summary>
    public static class DnnStaticDi
    {
        public static IServiceCollection StaticServiceCollection = null;

        public static void StaticDiReady() 
            => _sp = StaticServiceCollection?.BuildServiceProvider()
            ?? throw new Exception("Can't start Static DI for old Dnn, because the ServiceCollection is null.");

        /// <summary>
        /// Dependency Injection resolver with a known type as a parameter.
        /// </summary>
        /// <typeparam name="T">The type / interface we need.</typeparam>
        public static T Resolve<T>()
        {
            // TODO: DOCS FOR THIS
            throw new NotSupportedException("The Eav.Factory is obsolete. TODO: DOCS");
        }

        /// <summary>
        /// This is a special internal resolver for static objects
        /// Should only be used with extreme caution, as downstream objects
        /// May need more scope-specific stuff, why may be missing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// Avoid using at all cost - only DNN and test-code may use this!
        /// </remarks>
        [PrivateApi]
        public static T StaticBuild<T>() => GetServiceProvider().Build<T>();


        /// <summary>
        /// Dictionary key for keeping the Scoped Injection Service Provider in the Http-Context
        /// </summary>
        // we changed key to one used in DNN9DI instead old one "eav-scoped-serviceprovider"
        private static Type ServiceProviderKey = typeof(IServiceScope); // "eav-scoped-serviceprovider";

        private static IServiceProvider _sp;

        [PrivateApi("This is just a temporary solution - shouldn't be used long term")]
        private static IServiceProvider GetServiceProvider()
        {
            // Because 2sxc runs inside DNN as a webforms project and not asp.net core mvc, we have
            // to make sure the service-provider object is disposed correctly. If we don't do this,
            // connections to the database are kept open, and this leads to errors like "SQL timeout:
            // "All pooled connections were in use". https://github.com/2sic/2sxc/issues/1200
            // Work-around for issue https://github.com/2sic/2sxc/issues/1200
            // Scope service-provider based on request
            var httpCtx = HttpContext.Current;
            if (httpCtx == null) return _sp.CreateScope().ServiceProvider;

            // This only runs in Dnn 7.4.2 - Dnn 9.3, because Dnn 9.4 provides this in the http context
            if (httpCtx.Items[ServiceProviderKey] == null)
            {
                httpCtx.Items[ServiceProviderKey] = _sp.CreateScope().ServiceProvider;

                // Make sure service provider is disposed after request finishes
                httpCtx.AddOnRequestCompleted(context =>
                {
                    ((IDisposable)context.Items[ServiceProviderKey])?.Dispose();
                });
            }

            return (IServiceProvider)httpCtx.Items[ServiceProviderKey];
        }
    }
}
