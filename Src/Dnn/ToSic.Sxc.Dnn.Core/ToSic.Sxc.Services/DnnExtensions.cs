using DotNetNuke.Common.Extensions;
using System.Web;
using ToSic.Eav.DI;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Provides extension methods for DNN Skins (Themes) and Modules.
    /// It only exists on the Dnn platform, not on Oqtane.
    /// </summary>
    /// <remarks>
    /// Created v14
    /// </remarks>
    [PublicApi]
    public static class DnnExtensions
    {
        /// <summary>
        /// Helper extension method to get a scoped service on a skin/theme or module.
        ///
        /// To call it you must prefix it with `this.` like `this.GetScopedService...`
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skinOrModule"></param>
        /// <returns></returns>
        /// <remarks>
        /// This requires an `HttpContext.Current` to exist, so it will not work within a search controller.
        ///
        /// History
        /// - Created in v14
        /// </remarks>
        public static T GetScopedService<T>(this System.Web.UI.UserControl skinOrModule) => GetScopedService<T>();

        /// <summary>
        /// Get a service from the current HTTP Scope.
        /// This is the standalone method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// A service of type `T` - if it can be found. Otherwise it will throw an error.
        /// </returns>
        /// <remarks>
        /// This requires an `HttpContext.Current` to exist, so it will not work within a search controller.
        ///
        /// History
        /// - Created in v14
        /// </remarks>
        public static T GetScopedService<T>() => HttpContext.Current.GetScope().ServiceProvider.Build<T>();
    }
}
