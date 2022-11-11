using System;
using System.Web;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Dnn.WebApi.HttpJson;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.Dnn.WebApi
{
    [DnnLogWebApi, JsonOnlyResponse]
    public abstract class DnnApiControllerWithFixes<TRealController> : DnnApiController, IHasLog where TRealController : class, IHasLog
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        internal const string DnnSupportedModuleNames = "2sxc,2sxc-app";

        protected DnnApiControllerWithFixes(string logSuffix) 
	    {
            Log = new Log("Api." + logSuffix, null, $"Path: {HttpContext.Current?.Request.Url.AbsoluteUri}");
            TimerWrapLog = Log.Fn(message: "timer", startTimer: true);
	        
            // ReSharper disable VirtualMemberCallInConstructor
            GetService<LogHistory>().Add(HistoryLogGroup ?? EavWebApiConstants.HistoryNameWebApi, Log);
            // ReSharper restore VirtualMemberCallInConstructor
        }

        // ReSharper disable once InconsistentNaming
        private readonly LogCall TimerWrapLog;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var callLog = Log.Fn();
            // Add the logger to the request, in case it's needed in error-reporting
	        controllerContext.Request.Properties.Add(DnnConstants.EavLogKey, Log);
	        base.Initialize(controllerContext);
            callLog.Done();
        }

        protected override void Dispose(bool disposing)
        {
            TimerWrapLog.Done();
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public ILog Log { get; }

        /// <summary>
        /// The group name for log entries in insights.
        /// Helps group various calls by use case. 
        /// </summary>
        protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

        /// <summary>
        ///  Extend Time so Web Server doesn't time out
        /// </summary>
        protected void PreventServerTimeout300() => HttpContext.Current.Server.ScriptTimeout = 300;

        /// <inheritdoc />
        public virtual TService GetService<TService>() => (_serviceProvider ?? (_serviceProvider = DnnStaticDi.GetPageScopedServiceProvider())).Build<TService>();
        // Must cache it, to be really sure we use the same ServiceProvider in the same request
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// The RealController which is the full backend of this controller.
        /// Note that it's not available at construction time, because the ServiceProvider isn't ready till later.
        /// </summary>
        protected virtual TRealController Real
            => _real ?? (_real = GetService<TRealController>().Init(Log)
                                 ?? throw new Exception($"Can't use {nameof(Real)} for unknown reasons"));
        private TRealController _real;

    }
}