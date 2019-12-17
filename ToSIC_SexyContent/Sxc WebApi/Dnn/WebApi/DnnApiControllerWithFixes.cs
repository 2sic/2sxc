using System;
using System.Web;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Helpers;
using ToSic.SexyContent.Environment;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi
{
    [WebApiLogDetails, JsonResponse]
    public abstract class DnnApiControllerWithFixes: DnnApiController, IHasLog
    {
        protected IAppEnvironment Env;

        protected DnnApiControllerWithFixes() 
	    {
            // ensure that the sql connection string is correct
            // this is technically only necessary, when dnn just restarted and didn't already set this
            Settings.EnsureSystemIsInitialized();

	        // ensure that the call to this webservice doesn't reset the language in the cookie
	        // this is a dnn-bug
	        Helpers.RemoveLanguageChangingCookie();

            Log = new Log("DNN.WebApi", null, $"Path: {HttpContext.Current?.Request?.Url?.AbsoluteUri}");
            TimerWrapLog = Log.Call(message: "timer", useTimer: true);
	        
            // ReSharper disable VirtualMemberCallInConstructor
	        if (LogHistoryName != null)
	            History.Add(LogHistoryName, Log);
            // ReSharper restore VirtualMemberCallInConstructor

            Env = new DnnEnvironment(Log);
        }

        // ReSharper disable once InconsistentNaming
        private readonly Action<string> TimerWrapLog;

        protected override void Initialize(HttpControllerContext controllerContext)
	    {
            // Add the logger to the request, in case it's needed in error-reporting
	        controllerContext.Request.Properties.Add(Constants.EavLogKey, Log);
	        base.Initialize(controllerContext);
        }

        protected override void Dispose(bool disposing)
        {
            TimerWrapLog(null);
            base.Dispose(disposing);
        }


        public ILog Log { get; }

        protected virtual string LogHistoryName { get; } = "web-api";
    }
}