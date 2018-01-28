using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ContentImportController : SxcApiController
	{
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext);
	        Log.Rename("2sApiC");
	        _eavCtc = new Eav.WebApi.ContentImportController(Log);
	    }

        private Eav.WebApi.ContentImportController _eavCtc;

        [HttpPost]
        public ContentImportResult EvaluateContent(ContentImportArgs args) => _eavCtc.EvaluateContent(args);


	    [HttpPost]
        public ContentImportResult ImportContent(ContentImportArgs args) => _eavCtc.ImportContent(args);

	    [HttpPost]
        public bool Import(EntityImport args) => _eavCtc.Import(args);

	}
}