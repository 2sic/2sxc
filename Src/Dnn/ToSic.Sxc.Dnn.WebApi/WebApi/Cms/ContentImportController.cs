using System.Web.Http;
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
        protected override string HistoryLogName => "Api.ConImp";

        [HttpPost]
        public ContentImportResultDto EvaluateContent(ContentImportArgsDto args) 
            => new Eav.WebApi.ContentImportApi(Log).EvaluateContent(args);


	    [HttpPost]
        public ContentImportResultDto ImportContent(ContentImportArgsDto args) 
            => new Eav.WebApi.ContentImportApi(Log).ImportContent(args);

	    [HttpPost]
        public bool Import(EntityImportDto args) 
            => new Eav.WebApi.ContentImportApi(Log).Import(args);
    }
}