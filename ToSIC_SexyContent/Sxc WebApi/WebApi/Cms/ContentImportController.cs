using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ContentImportController : SxcApiControllerBase, IContentImportController
    {
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext);
	        Log.Rename("2sApiC");
	        _eavCtc = new Eav.WebApi.ContentImportController(Log);
	    }

        private Eav.WebApi.ContentImportController _eavCtc;

        [HttpPost]
        public ContentImportResultDto EvaluateContent(ContentImportArgsDto args) => _eavCtc.EvaluateContent(args);


	    [HttpPost]
        public ContentImportResultDto ImportContent(ContentImportArgsDto args) => _eavCtc.ImportContent(args);

	    [HttpPost]
        public bool Import(EntityImportDto args) => _eavCtc.Import(args);

	}
}