using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
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
}
