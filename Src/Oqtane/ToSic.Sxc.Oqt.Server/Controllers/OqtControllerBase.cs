using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Lib;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared.Dev;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    /// <summary>
    /// Api controllers normally should inherit ControllerBase but we have a special case of inhering from Controller.
    /// It is because our custom dynamic 2sxc app api controllers (without constructor), depends on event OnActionExecuting
    /// to provide dependencies (without DI in constructor).
    /// </summary>
    [SystemTestJsonFormatter] // This is needed to preserve compatibility with previous api usage
    [ServiceFilter(typeof(OptionalBodyFilter))] // Instead of global options.AllowEmptyInputInBodyModelBinding = true;
    [ServiceFilter(typeof(HttpResponseExceptionFilter))]
    public abstract class OqtControllerBase<TRealController> : Controller, IHasLog where TRealController : class, IHasLog
    {
        protected OqtControllerBase(string logSuffix)
        {
            Log = new Log($"Api.{logSuffix}", null, GetType().Name);
            _helper = new(this);
        }

        protected IServiceProvider ServiceProvider => _helper.ServiceProvider;

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
        /// Initializer - just ensure SiteState is initialized thanks to our paths
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _helper.OnActionExecuting(context, HistoryLogGroup);

            // background processes can pass in an alias using the SiteState service
            GetService<SiteStateInitializer>().InitIfEmpty();
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

        protected TService GetService<TService>() where TService : class => _helper.GetService<TService>();

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        protected virtual TRealController Real => _real.Get(() => _helper.Real<TRealController>()) ;
        private readonly GetOnce<TRealController> _real = new();
    }
}
