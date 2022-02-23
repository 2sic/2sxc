using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Helpers;
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
            _helper = new NetCoreControllersHelper(this);
        }

        protected IServiceProvider ServiceProvider;

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The helper to assist in timing and common operations of WebApi Controllers
        /// </summary>
        private readonly NetCoreControllersHelper _helper;


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

        //private Action<string> ActionTimerWrap; // it is used across events to track action execution total time

        /// <summary>
        /// Initializer - just ensure SiteState is initialized thanks to our paths
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ServiceProvider = _helper.OnActionExecuting(context, HistoryLogGroup);

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
            _helper.OnActionExecuted(context);
        }
    }


}
