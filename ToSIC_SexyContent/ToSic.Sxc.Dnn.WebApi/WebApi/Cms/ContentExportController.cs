using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.Security;

namespace ToSic.Sxc.WebApi.Cms
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
    /// Security checking is possible, because the cookie still contains user information
    /// </remarks>
    [AllowAnonymous]
    [DnnLogExceptions]
    // [ValidateAntiForgeryToken] as these pages are called directly (browser opens the url directly to download files), we
    // can't globally use this attribute.
    public class ContentExportController : DnnApiControllerWithFixes, IContentExportController
    {
        protected override string HistoryLogName => "Api.2sSysC";

	    private Eav.WebApi.ContentExportApi EavCtc => _eavCtc ?? (_eavCtc = new Eav.WebApi.ContentExportApi(Log));

	    private Eav.WebApi.ContentExportApi _eavCtc;


        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage ExportContent(
            int appId, 
            string language, 
            string defaultLanguage, 
            string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            Log.Add($"export content start app:{appId}, language:{language}, defLang:{defaultLanguage}, type:{contentType}, ids:{selectedIds}");
            // do security check and get data
            return RunIf.Admin(PortalSettings, () => EavCtc.ExportContent(appId, language, defaultLanguage, contentType,
                    recordExport, resourcesReferences,
                    languageReferences, selectedIds));
        }

	    [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadTypeAsJson(int appId, string name) 
            => RunIf.Admin(PortalSettings,() => EavCtc.DownloadTypeAsJson(appId, name));

        [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadEntityAsJson(int appId, int id, string prefix, bool withMetadata)
            => RunIf.Admin(PortalSettings, () => EavCtc.DownloadEntityAsJson(appId, id, prefix, withMetadata));

    }
}