using System;
using System.Web;
using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.WebApi.Helpers;
using ToSic.SexyContent.Environment;

namespace ToSic.SexyContent.WebApi.Dnn
{
    [WebApiLogDetails, JsonResponse]
    public class DnnApiControllerWithFixes: DnnApiController, IHasLog
    {
        protected IAppEnvironment Env;

	    public DnnApiControllerWithFixes() 
	    {
            // ensure that the sql connection string is correct
            // this is technically only necessary, when dnn just restarted and didn't already set this
            Settings.EnsureSystemIsInitialized();

	        // ensure that the call to this webservice doesn't reset the language in the cookie
	        // this is a dnn-bug
	        Helpers.RemoveLanguageChangingCookie();

            Log = new Log("DNN.WebApi", null, $"Path: {HttpContext.Current?.Request?.Url?.AbsoluteUri}");
	        
            // ReSharper disable VirtualMemberCallInConstructor
	        if (LogHistorySetName != null)
	            History.Add(LogHistorySetName, Log);
            // ReSharper restore VirtualMemberCallInConstructor

            Env = new DnnEnvironment(Log);
        }

        protected override void Initialize(HttpControllerContext controllerContext)
	    {
            // Add the logger to the requst, in case it's needed in error-reporting
	        controllerContext.Request.Properties.Add(Constants.EavLogKey, Log);
	        base.Initialize(controllerContext);
        }


        public ILog Log { get; }

        public void LinkLog(ILog parentLog)
        {
            throw new NotImplementedException();
        }

        protected virtual string LogHistorySetName { get; set; } = "web-api";
    }
}