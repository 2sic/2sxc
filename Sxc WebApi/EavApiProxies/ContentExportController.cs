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
    public class ContentExportController : DnnApiControllerWithFixes 
	{
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("2sSysC");
            _eavCtc = new Eav.WebApi.ContentExportController(Log);
	    }

	    private Eav.WebApi.ContentExportController _eavCtc;


        [HttpGet]
        [AllowAnonymous] // will do security check internall
        public HttpResponseMessage ExportContent(int appId, string language, string defaultLanguage, string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            Log.Add($"export content start app:{appId}, language:{language}, defLang:{defaultLanguage}, type:{contentType}, ids:{selectedIds}");
            // do security check
            if(!PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                throw new HttpRequestException("Needs admin permissions to do this");

            Log.Add("export content starting");
            return _eavCtc.ExportContent(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
                languageReferences, selectedIds);
        }


    }
}