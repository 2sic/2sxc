using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Entities.Content;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.WebApi;
using static ToSic.Eav.WebApi.ContentImportController;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ContentImportController : SxcApiController
	{
        private readonly Eav.WebApi.ContentImportController eavCtc;
        public ContentImportController()
        {
            eavCtc = new Eav.WebApi.ContentImportController();
            eavCtc.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

        }


        [HttpPost]
        public ContentImportResult EvaluateContent(ContentImportArgs args)
        {
            return eavCtc.EvaluateContent(args);
        }


        [HttpPost]
        public ContentImportResult ImportContent(ContentImportArgs args)
        {
            return eavCtc.ImportContent(args);

        }


    }
}