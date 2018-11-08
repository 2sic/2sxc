using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.SexyContent.WebApi.Dnn;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    /// <summary>
    /// Web API Controller for the Pipeline Designer UI
    /// </summary>
    /// <remarks>
    /// Because these calls are made in a new window, they won't contain any http-headers like module-id or security token. 
    /// So we can't use the classic protection attributes like:
    /// - [SupportedModules("2sxc,2sxc-app")]
    /// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Anonymous)]
    /// Instead, the method itself must do additional security checking.
    /// Security checking is possible, becauset the cookie still contains user information
    /// </remarks>
    [AllowAnonymous]
    [SxcWebApiExceptionHandling]
    public class ContentExportController : DnnApiControllerWithFixes, IContentExportController
    {
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
	        Log.Rename("2sSysC");
            _eavCtc = new Eav.WebApi.ContentExportController(Log);
	    }

	    private Eav.WebApi.ContentExportController _eavCtc;


        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage ExportContent(int appId, string language, string defaultLanguage, string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            Log.Add($"export content start app:{appId}, language:{language}, defLang:{defaultLanguage}, type:{contentType}, ids:{selectedIds}");
            // do security check and get data
            return PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName)
                ? _eavCtc.ExportContent(appId, language, defaultLanguage, contentType,
                    recordExport, resourcesReferences,
                    languageReferences, selectedIds)
                : throw new HttpRequestException("Needs admin permissions to do this");
        }

	    [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadTypeAsJson(int appId, string name)
	        => PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName)
	            ? _eavCtc.DownloadTypeAsJson(appId, name)
	            : throw new HttpRequestException("Needs admin permissions to do this");

	    [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadEntityAsJson(int appId, int id, string prefix, bool withMetadata)
            => PortalSettings.UserInfo.IsInRole(PortalSettings.AdministratorRoleName)
	            ? _eavCtc.DownloadEntityAsJson(appId, id, prefix, withMetadata)
	            : throw new HttpRequestException("Needs admin permissions to do this");

    }
}