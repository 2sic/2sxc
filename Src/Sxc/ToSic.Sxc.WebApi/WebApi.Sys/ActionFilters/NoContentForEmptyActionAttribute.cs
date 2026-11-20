#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToSic.Sxc.WebApi.Sys.ActionFilters;

/// <summary>Preserves HTTP 204 for successful actions returning void or a non-generic Task.</summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class NoContentForEmptyActionAttribute : ActionFilterAttribute
{
    public NoContentForEmptyActionAttribute() => Order = int.MinValue + 2;

    public override void OnActionExecuted(ActionExecutedContext context)
        => ApplyNoContentStatus(context);

    internal static void ApplyNoContentStatus(ActionExecutedContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor action
            || context.HttpContext.Response.StatusCode != StatusCodes.Status200OK)
            return;

        // Some clients try to parse an empty HTTP 200 response as JSON.
        var returnType = action.MethodInfo.ReturnType;
        if (returnType == typeof(void) || returnType == typeof(Task))
            context.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
    }
}
#endif
