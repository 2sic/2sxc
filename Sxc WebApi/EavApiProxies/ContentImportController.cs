using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using static ToSic.Eav.WebApi.ContentImportController;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ContentImportController : SxcApiController
	{
        private readonly Eav.WebApi.ContentImportController _eavCtc = new Eav.WebApi.ContentImportController();
        //public ContentImportController()
        //{
        //    // now uses dependency injection
        //    //_eavCtc = new Eav.WebApi.ContentImportController();
        //    //_eavCtc.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

        //}


        [HttpPost]
        public ContentImportResult EvaluateContent(ContentImportArgs args) => _eavCtc.EvaluateContent(args);


	    [HttpPost]
        public ContentImportResult ImportContent(ContentImportArgs args) => _eavCtc.ImportContent(args);
	}
}