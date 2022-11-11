using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Helpers;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    /// <summary>
    /// This kind of base controller is recommended for all 2sxc/eav implementations. It solves various problems:
    ///
    /// 1. Ensures logging and also times the request for better insights
    /// 2. Provides a `GetService` command for all API Controllers
    /// 3. Provides the `Real` controller which was specified
    /// 4. Standardizes the response ni case of a empty response so it properly reports HTTP 204
    /// </summary>
    /// <typeparam name="TRealController"></typeparam>
    public abstract class IntControllerBase<TRealController> : Controller, IHasLog where TRealController : class, IHasLog
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        /// <summary>
        /// Constructor initializes the log, and makes sure that the helper is ready for timing and other features
        /// </summary>
        /// <param name="logName"></param>
        protected IntControllerBase(string logName)
        {
            Log = new Log(IntegrationConstants.LogPrefix + logName, null, GetType().Name);
            _helper = new NetCoreControllersHelper(this);
        }

        /// <summary>
        /// The helper to assist in timing and common operations of WebApi Controllers
        /// </summary>
        private readonly NetCoreControllersHelper _helper;

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case. 
        /// </summary>
        protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

        /// <summary>
        /// Initializer - Prepare the ServiceProvider and start the timer
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            _helper.OnActionExecuting(context, HistoryLogGroup);
        }

        /// <summary>
        /// Make sure we stop the timer and do some minor fixes
        /// </summary>
        /// <param name="context"></param>
        [NonAction]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            _helper.OnActionExecuted(context);
        }

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        protected TRealController Real => _real ??= _helper.Real<TRealController>();
        private TRealController _real;

    }
}
