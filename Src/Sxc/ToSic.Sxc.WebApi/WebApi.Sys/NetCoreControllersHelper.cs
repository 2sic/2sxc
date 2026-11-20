#if !NETFRAMEWORK
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.WebApi.Sys.ActionFilters;

namespace ToSic.Sxc.WebApi.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class NetCoreControllersHelper(ControllerBase parent) : ICanGetService
{
    public ControllerBase Parent { get; } = parent;
    public ILog? LogOrNull { get; } = (parent as IHasLog)?.Log;

    private ILogCall? _actionTimerWrap; // it is used across events to track action execution total time

    public IServiceProvider ServiceProvider => _serviceProvider ?? throw new($"{nameof(ServiceProvider)} is only available after calling {nameof(OnActionExecuting)}");
    private IServiceProvider? _serviceProvider;


    public void OnActionExecuting(ActionExecutingContext context, string historyLogGroup)
    {
        // Create a log entry with timing
        _actionTimerWrap = LogOrNull.Fn($"action executing url: {context.HttpContext.Request.GetDisplayUrl()}", timer: true);

        // Get the ServiceProvider of the current request
        _serviceProvider = context.HttpContext.RequestServices;

        // Add to Log History
        if (LogOrNull != null)
            GetService<ILogStore>().Add(historyLogGroup, LogOrNull);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        NoContentForEmptyActionAttribute.ApplyNoContentStatus(context);

        _actionTimerWrap.Done("ok");
        _actionTimerWrap = null; // just to mark that Action Delegate is not in use any more, so GC can collect it
    }

    public TService GetService<TService>() where TService : class => ServiceProvider.Build<TService>(LogOrNull);
}

#endif 