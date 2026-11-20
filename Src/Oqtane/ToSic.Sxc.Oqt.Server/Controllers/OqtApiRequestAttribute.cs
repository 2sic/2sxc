using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Oqt.Server.Controllers;

/// <summary>Runs the common Oqtane request lifecycle for an internal API controller.</summary>
internal sealed class OqtApiRequestAttribute : TypeFilterAttribute
{
    public OqtApiRequestAttribute(string historyLogGroup = EavWebApiConstants.HistoryNameWebApi)
        : base(typeof(OqtApiRequestFilter))
    {
        Arguments = [historyLogGroup];
        // Match the old controller filter's position outside the compatibility filters.
        Order = int.MinValue;
    }
}

internal sealed class OqtApiRequestFilter(string historyLogGroup) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var helper = new NetCoreControllersHelper((ControllerBase)context.Controller);
        helper.OnActionExecuting(context, historyLogGroup);

        // SiteState can already carry an alias supplied by a background caller.
        helper.GetService<AliasResolver>().InitIfEmpty();

        helper.OnActionExecuted(await next());
    }
}
