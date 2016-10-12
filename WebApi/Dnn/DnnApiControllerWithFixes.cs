using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.Dnn
{
	public class DnnApiControllerWithFixes: DnnApiController
	{
	    public DnnApiControllerWithFixes()
	    {
            // ensure that the sql connection string is correct
            // this is technically only necessary, when dnn just restarted and didn't already set this
            Settings.SetEavConnectionString();

            // ensure that the call to this webservice doesn't reset the language in the cookie
            // this is a dnn-bug
            Helpers.RemoveLanguageChangingCookie();
        }


    }
}