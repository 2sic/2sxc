using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.ImportExport.Refactoring.Options;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    // [SupportedModules("2sxc,2sxc-app")]
    
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    [AllowAnonymous]
    [SxcWebApiExceptionHandling]
    public class ContentExportController : DnnApiControllerWithFixes // DnnApiController // SxcApiController
	{
        private readonly Eav.WebApi.ContentExportController eavCtc;
        public ContentExportController()
        {
            eavCtc = new Eav.WebApi.ContentExportController();
            eavCtc.SetUser(Environment.Dnn7.UserIdentity.CurrentUserIdentityToken);

        }


        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ExportContent(int appId, string language, string defaultLanguage, string contentType,
            RecordExport recordExport, ResourceReferenceExport resourcesReferences,
            LanguageReferenceExport languageReferences, string selectedIds = null)
        {
            // do security check
            if(!PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName))// todo: copy to 8.5 "Administrators")) // note: user.isinrole didn't work
                throw new HttpRequestException("Needs admin permissions to do this");
            return eavCtc.ExportContent(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
                languageReferences, selectedIds);
        }


    }
}