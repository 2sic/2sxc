using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.ImportExport.Options;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi.EavApiProxies
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
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("2sSysC");
            _eavCtc = new Eav.WebApi.ContentExportController();
	    }

	    private Eav.WebApi.ContentExportController _eavCtc;


        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ExportContent(int appId, string language, string defaultLanguage, string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            // do security check
            if(!PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName))// todo: copy to 8.5 "Administrators")) // note: user.isinrole didn't work
                throw new HttpRequestException("Needs admin permissions to do this");
            return _eavCtc.ExportContent(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
                languageReferences, selectedIds);
        }


    }
}