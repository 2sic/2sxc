#if NETCOREAPP
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.ActionFilters;
/// <summary>
/// TODO: @STV pls document what this is for
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        else if (context.Exception is Exception anyException)
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