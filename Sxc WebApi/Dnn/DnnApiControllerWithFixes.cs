using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Environment;

namespace ToSic.SexyContent.WebApi.Dnn
{
    [WebApiLogDetails]
    public class DnnApiControllerWithFixes: DnnApiController
    {
        protected IEnvironment Env;
        protected Log Log;

	    public DnnApiControllerWithFixes()
	    {
            // ensure that the sql connection string is correct
            // this is technically only necessary, when dnn just restarted and didn't already set this
            Settings.EnsureSystemIsInitialized();

	        // ensure that the call to this webservice doesn't reset the language in the cookie
	        // this is a dnn-bug
	        Helpers.RemoveLanguageChangingCookie();

            Log = new Log("DN.WebApi");
            Env = new DnnEnvironment(Log);
        }

        protected override void Initialize(HttpControllerContext controllerContext)
	    {
            // Add the logger to the requst, in case it's needed in error-reporting
	        controllerContext.Request.Properties.Add(Constants.EavLogKey, Log);
	        base.Initialize(controllerContext);
        }


    }
}