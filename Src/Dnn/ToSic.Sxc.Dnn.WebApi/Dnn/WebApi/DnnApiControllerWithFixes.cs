using System;
using System.Web;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.Dnn.WebApi
{
    [DnnLogWebApi, JsonResponse]
    public abstract class DnnApiControllerWithFixes: DnnApiController, IHasLog
    {
        protected DnnApiControllerWithFixes() 
	    {
            // ReSharper disable once VirtualMemberCallInConstructor
            Log = new Log(HistoryLogName, null, $"Path: {HttpContext.Current?.Request?.Url?.AbsoluteUri}");
            TimerWrapLog = Log.Call(message: "timer", useTimer: true);
	        
            // ReSharper disable VirtualMemberCallInConstructor
	        if (HistoryLogGroup != null)
                Eav.Factory.StaticBuild<LogHistory>().Add(HistoryLogGroup, Log);
            // ReSharper restore VirtualMemberCallInConstructor

        }

        // ReSharper disable once InconsistentNaming
        private readonly Action<string> TimerWrapLog;

        protected override void Initialize(HttpControllerContext controllerContext)
	    {
            // Add the logger to the request, in case it's needed in error-reporting
	        controllerContext.Request.Properties.Add(DnnConstants.EavLogKey, Log);
	        base.Initialize(controllerContext);
        }

        protected override void Dispose(bool disposing)
        {
            TimerWrapLog(null);
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
        /// The name of the logger in insights.
        /// The inheriting class should provide the real name to be used.
        /// </summary>
        protected abstract string HistoryLogName { get; }


        /// <summary>
        ///  Extend Time so Web Server doesn't time out
        /// </summary>
        protected void PreventServerTimeout300() => HttpContext.Current.Server.ScriptTimeout = 300;

        /// <inheritdoc />
        public TService GetService<TService>() => Eav.Factory.StaticBuild<TService>();

    }
}