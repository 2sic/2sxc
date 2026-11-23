using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi;


/// <summary>
/// Middleware for handling requests to the custom App API.
/// - extract route values like appFolder, controller, action, apiFile, dllName, etc.
/// - compile controller if not already compiled
/// - ensure security, 
/// - provide action context and invoke controller action
/// </summary>
internal static class AppApiMiddleware
{
  /// <summary>
  /// Invokes the middleware asynchronously.
  /// </summary>
  /// <param name="context">The HTTP context.</param>
  [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
  internal static async Task InvokeAsync(HttpContext context)
  {
    // Transform route values.
    var sp = context.GetSysRequestServices();

    var appApiDynamicRouteValueTransformer = sp.GetService<AppApiDynamicRouteValueTransformer>();
    var values = await appApiDynamicRouteValueTransformer.TransformAsync(context, context.Request.RouteValues);

    // Compile and register dyncode app api controller.
    var appApiControllerManager = sp.GetService<AppApiControllerManager>();
    if (!await appApiControllerManager.PrepareController(values))
      throw new KeyNotFoundException("Error, can't compile controller.");

    // Provide ActionContext.
    var appApiActionContext = sp.GetService<AppApiActionContext>();
    var actionContext = appApiActionContext.Provide(context, values);

    async Task InvokeActionAfterAuthorization(HttpContext context)
    {
      // Invoke controller action.
      var appApiActionInvoker = sp.GetService<AppApiActionInvoker>();
      await appApiActionInvoker.Invoke(actionContext);
    }

    // Check security.
    var appApiAuthorization = sp.GetService<AppApiAuthorization>().Init(InvokeActionAfterAuthorization);
    await appApiAuthorization.Invoke(actionContext);
  }
}
