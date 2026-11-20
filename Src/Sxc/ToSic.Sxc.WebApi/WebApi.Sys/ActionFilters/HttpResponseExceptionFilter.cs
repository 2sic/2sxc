#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToSic.Sxc.WebApi.Sys.ActionFilters;
/// <summary>
/// Converts exceptions from ASP.NET Core Web API actions into HTTP responses for 2sxc clients.
/// </summary>
/// <remarks>
/// An <see cref="HttpExceptionAbstraction"/> supplies the response status and message.
/// Other exceptions produce HTTP 400 with the exception as the response value,
/// preserving the Web API error contract.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpExceptionAbstraction exception)
        {
            context.Result = new ObjectResult(exception.Status)
            {
                StatusCode = exception.Status,
                Value = exception.Message,
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception is { } anyException)
        {
            context.Result = new ObjectResult(500)
            {
                StatusCode = 400,
                Value = anyException,
            };
            context.ExceptionHandled = true;
        }   
    }
}
#endif
