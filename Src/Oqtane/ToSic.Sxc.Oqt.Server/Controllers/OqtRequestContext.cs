using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal static class OqtRequestContext
{
    /// <summary>Builds and publishes the block for this request when module and page IDs are available.</summary>
    internal static IContextCompleteService PrepareRequestBlock(this ControllerBase controller)
        => controller.HttpContext.RequestServices.Build<IWebApiContextBuilder>((controller as IHasLog)?.Log)
            .PrepareContextResolverForApiRequest();
}

/// <summary>Prepares the optional block when every action on a controller uses it.</summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class OqtPrepareBlockContextAttribute : ActionFilterAttribute
{
    public OqtPrepareBlockContextAttribute() => Order = int.MinValue + 1;

    public override void OnActionExecuting(ActionExecutingContext context)
        => ((ControllerBase)context.Controller).PrepareRequestBlock();
}
