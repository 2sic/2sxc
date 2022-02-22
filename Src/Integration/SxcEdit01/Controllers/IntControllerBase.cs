using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    public abstract class IntControllerBase : Controller, IHasLog
    {
        protected IntControllerBase(string logName)
        {
            Log = new Log(IntegrationConstants.LogPrefix + logName, null, GetType().Name);
        }

        /// <inheritdoc />
        public ILog Log { get; }

        protected IServiceProvider ServiceProvider;

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case. 
        /// </summary>
        protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

        private Action<string> ActionTimerWrap; // it is used across events to track action execution total time

        /// <summary>
        /// Initializer - just ensure SiteState is initialized thanks to our paths
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ActionTimerWrap = Log.Call($"action executing url: {context.HttpContext.Request.GetDisplayUrl()}", useTimer: true);

            base.OnActionExecuting(context);

            ServiceProvider = context.HttpContext.RequestServices;

            // add log
            ServiceProvider.Build<LogHistory>().Add(HistoryLogGroup, Log);
        }

        /// <inheritdoc/>
        [NonAction]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                // If the api endpoint method return type is "void" or "Task", Web API will return HTTP response with status code 204 (No Content).
                // This changes aspnetcore default behavior in Oqtane that returns HTTP 200 OK, with no body so it is same as in ASP.NET MVC2 in DNN. 
                // This is helpful for jQuery Ajax issue that on HTTP 200 OK with empty body throws json parse error.
                // https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/action-results#void
                // https://github.com/dotnet/aspnetcore/issues/16944
                // https://github.com/2sic/2sxc/issues/2555
                var returnType = actionDescriptor.MethodInfo.ReturnType;
                if (returnType == typeof(void) || returnType == typeof(Task))
                {
                    if (context.HttpContext.Response.StatusCode == 200)
                        context.HttpContext.Response.StatusCode = 204; // NoContent (instead of HTTP 200 OK)
                }
            }

            ActionTimerWrap("ok");
            ActionTimerWrap = null; // just to mark that Action Delegate is not in use any more, so GC can collect it
        }

    }

}
