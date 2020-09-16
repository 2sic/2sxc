using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

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

        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage ExportContent(
            int appId, 
            string language, 
            string defaultLanguage, 
            string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null) =>
            new Eav.WebApi.ContentExportApi(Log).ExportContent(
                new DnnUser(UserInfo), appId,
                language, defaultLanguage, contentType,
                recordExport, resourcesReferences,
                languageReferences, selectedIds);

        [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadTypeAsJson(int appId, string name) 
            => new Eav.WebApi.ContentExportApi(Log).DownloadTypeAsJson(new DnnUser(UserInfo),  appId, name);

        [HttpGet]
	    [AllowAnonymous] // will do security check internally
	    public HttpResponseMessage DownloadEntityAsJson(int appId, int id, string prefix, bool withMetadata)
            => new Eav.WebApi.ContentExportApi(Log).DownloadEntityAsJson(new DnnUser(UserInfo), appId, id, prefix, withMetadata);

    }
}