using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.Dnn
{
	public class DnnApiControllerWithFixes: DnnApiController
	{
	    public DnnApiControllerWithFixes()
	    {
            Helpers.RemoveLanguageChangingCookie();
        }


    }
}