using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Usage;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Provide information about views and manage views as needed.
    /// This should also work on Oqtane once released May the 4th 2021 :)
    /// </summary>
    /// <remarks>
    /// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
    /// So we can't use the classic protection attributes to the class like:
    /// - [SupportedModules("2sxc,2sxc-app")]
    /// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    /// - [ValidateAntiForgeryToken]
    /// Instead, each method must have all attributes, or do additional security checking.
    /// Security checking is possible, because the cookie still contains user information
    /// </remarks>
    [DnnLogExceptions]
    public class ViewController : SxcApiControllerBase
	{
        /// <summary>
        /// Name of this class in the insights logs.
        /// </summary>
        protected override string HistoryLogName => "Api.TmpCnt";

        private ViewsBackend Backend => GetService<ViewsBackend>().Init(Log);
        private ViewsExportImport ExportImport => GetService<ViewsExportImport>().Init(Log);

        /// <summary>
        /// Get the views of this App
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDetailsDto> All(int appId) => Backend.GetAll(appId);

        /// <summary>
        /// Find out how polymorphism is configured in this App.
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public PolymorphismDto Polymorphism(int appId) => GetService<PolymorphismBackend>().Init(Log).Polymorphism(appId);

        /// <summary>
        /// Delete a View
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, HttpDelete]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, int id) => Backend.Delete(appId, id);

        /// <summary>
        /// Download / export a view as JSON to easily re-import into another App.
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        /// <remarks>
        /// New in 2sxc 11.07
        /// </remarks>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int viewId) => ExportImport.DownloadViewAsJson(appId, viewId);

        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <remarks>
        /// New in 2sxc 11.07
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            if (HttpContext.Current.Request.Files.Count <= 0)
                return new ImportResultDto(false, "no file uploaded", Message.MessageTypes.Error);

            var files = HttpContext.Current.Request.Files;
            var streams = new List<FileUploadDto>();
            for (var i = 0; i < files.Count; i++)
                streams.Add(new FileUploadDto { Name = files[i].FileName, Stream = files[i].InputStream });
            var result = ExportImport.ImportView(zoneId, appId, streams, PortalSettings.DefaultLanguage);

            return wrapLog("ok", result);
        }

        /// <summary>
        /// Get usage statistics for entities so the UI can guide the user
        /// to find out if data is being used or if it can be safely deleted.
        /// </summary>
        /// <param name="appId">App ID</param>
        /// <param name="guid">Guid of the Entity</param>
        /// <returns></returns>
        /// <remarks>
        /// New in 2sxc 11.11
        /// </remarks>
        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid)
            => GetService<UsageBackend>().Init(Log)
                .ViewUsage(appId, guid, (views, blocks) =>
                    {
                        // create array with all 2sxc modules in this portal
                        var allMods = new Pages.Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
                        Log.Add($"Found {allMods.Count} modules");

                        return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
                    });

    }
}