using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Usage;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [AllowAnonymous] // necessary at this level, because otherwise download would fail
    public class ViewController : SxcApiControllerBase
	{
        protected override string HistoryLogName => "Api.TmpCnt";

        private ViewsBackend Backend => _build<ViewsBackend>().Init(new DnnSite(), new DnnUser(), Log);
        private ViewsExportImport exportImport => _build<ViewsExportImport>().Init(new DnnSite(), new DnnUser(), Log);

        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<object> All(int appId) => Backend.GetAll(appId);

        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public PolymorphismDto Polymorphism(int appId) => _build<PolymorphismBackend>().Init(Log).Polymorphism(appId);


        [HttpGet, HttpDelete]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, int id) => Backend.Delete(appId, id);

        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int viewId) => exportImport.DownloadViewAsJson(appId, viewId);

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
            var result = exportImport.ImportView(zoneId, appId, streams, PortalSettings.DefaultLanguage);

            return wrapLog("ok", result);
        }

        [HttpGet]
        [SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid)
            => _build<UsageBackend>().Init(Log)
                .ViewUsage(GetContext(), appId, guid,
                    (views, blocks) =>
                    {
                        // create array with all 2sxc modules in this portal
                        var allMods = new Pages.Pages(Log).AllModulesWithContent(PortalSettings.PortalId);
                        Log.Add($"Found {allMods.Count} modules");

                        return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
                    });

    }
}