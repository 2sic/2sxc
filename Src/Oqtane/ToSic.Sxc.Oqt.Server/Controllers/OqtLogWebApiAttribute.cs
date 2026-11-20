using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToSic.Sxc.Oqt.Server.Controllers;

/// <summary>Records and times an Oqtane API action in Insights.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class OqtLogWebApiAttribute(string historyLogGroup = EavWebApiConstants.HistoryNameWebApi)
    : Attribute, IAsyncActionFilter, IOrderedFilter
{
    public int Order => int.MinValue;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is not IHasLog { Log: { } log })
        {
            await next();
            return;
        }

        var http = context.HttpContext;
        var timer = log.Fn($"action executing url: {http.Request.GetDisplayUrl()}", timer: true);
        try
        {
            http.RequestServices.Build<ILogStore>(log).Add(historyLogGroup, log);
            await next();
        }
        finally
        {
            timer.Done();
        }
    }
}
