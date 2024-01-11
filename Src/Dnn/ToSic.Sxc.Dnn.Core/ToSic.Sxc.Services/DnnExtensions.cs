using System.Web;
using DotNetNuke.Common.Extensions;
using ToSic.Sxc.Dnn;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services;

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
    /// - In v15.03 added special patch so it work on 404 pages where the service provider is broken/not available
    /// </remarks>
    public static T GetScopedService<T>()
    {
        var serviceScope = HttpContext.Current.GetScope();
        // Check if the service scope is broken (typical on 404 pages) and do workaround
        // Should be removed once the minimum DNN is upgraded to a version where it is fixed
        // As of 2022-02-20 DNN 9.11.0 it's still broken
        return serviceScope == null
#pragma warning disable CS0618
            ? DnnStaticDi.StaticBuild<T>() // handles edge case for 404 page in DNN where scope is missing, fix https://github.com/2sic/2sxc/issues/2986
#pragma warning restore CS0618
            : serviceScope.ServiceProvider.Build<T>();
    }
}