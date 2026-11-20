using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToSic.Sxc.Oqt.Server.Controllers;

/// <summary>Records and times an Oqtane API action in Insights.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class OqtLogWebApiAttribute(string historyLogGroup = EavWebApiConstants.HistoryNameWebApi)
    : Attribute, IAsyncActionFilter, IOrderedFilter
{
    private static readonly object RequestLoggingKey = new();

    public int Order => int.MinValue;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;
        // The first filter owns logging even when the attribute is also declared on the action.
        if (context.Controller is not IHasLog owner || !http.Items.TryAdd(RequestLoggingKey, true))
        {
            await next();
            return;
        }

        ILogCall? timer = null;
        try
        {
            if (owner.Log is { } log)
            {
                timer = log.Fn($"action executing url: {http.Request.GetDisplayUrl()}", timer: true);
                http.RequestServices.Build<ILogStore>(log).Add(historyLogGroup, log);
            }
        }
        catch
        {
            // Insights is best-effort. A logging failure must still let the action run.
        }

        try
        {
            await next();
        }
        finally
        {
            try
            {
                timer.Done();
            }
            catch
            {
                // A logging failure must preserve the action result or its original exception.
            }
        }
    }
}
