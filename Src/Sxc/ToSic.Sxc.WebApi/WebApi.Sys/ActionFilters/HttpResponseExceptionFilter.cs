#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ToSic.Sxc.WebApi.Sys.ActionFilters;
/// <summary>
/// Converts exceptions from Oqtane controller actions into HTTP 400 responses,
/// using the exception as the response value.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order { get; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //if (context.Exception is Exception exception)
        //{
        //    context.Result = new ObjectResult(500)
        //    {
        //        StatusCode = 500,
        //        Value = exception.Message,
        //    };
        //    context.ExceptionHandled = true;
        //}
        //else 
        if (context.Exception is { } anyException)
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
