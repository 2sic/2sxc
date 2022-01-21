using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared.Dev;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Api controllers normally should inherit ControllerBase but we have a special case of inhering from Controller.
    /// It is because our custom dynamic 2sxc app api controllers (without constructor), depends on event OnActionExecuting
    /// to provide dependencies (without DI in constructor).
    /// </summary>
    [NewtonsoftJsonFormatter] // This is needed to preserve compatibility with previous api usage
    [ServiceFilter(typeof(OptionalBodyFilter))] // Instead of global options.AllowEmptyInputInBodyModelBinding = true;
    [ServiceFilter(typeof(HttpResponseExceptionFilter))]
    public abstract class OqtControllerBase : Controller, IHasLog
    {
        protected OqtControllerBase()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Log = new Log(HistoryLogName, null, GetType().Name);
        }

        protected IServiceProvider ServiceProvider;

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case.
        /// </summary>
        protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

        /// <summary>
        /// The name of the logger in insights.
        /// The inheriting class should provide the real name to be used.
        /// </summary>
        protected abstract string HistoryLogName { get; }

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

            // background processes can pass in an alias using the SiteState service
            ServiceProvider.Build<SiteStateInitializer>().InitIfEmpty();
        }

        #region Extend Time so Web Server doesn't time out - not really implemented ATM

        protected void PreventServerTimeout300() => WipConstants.DontDoAnythingImplementLater();

        #endregion

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
