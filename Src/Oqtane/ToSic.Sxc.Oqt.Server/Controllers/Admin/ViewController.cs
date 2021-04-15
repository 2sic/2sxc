using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    [AllowAnonymous] // necessary at this level, because otherwise download would fail
    [Route(WebApiConstants.WebApiStateRoot + "/admin/view/[action]")]
    public class ViewController : OqtStatefulControllerBase
    {
        private readonly Lazy<ViewsBackend> _viewsBackendLazy;
        private readonly Lazy<ViewsExportImport> _viewExportLazy;
        protected override string HistoryLogName => "Api.TmpCnt";

        private ViewsBackend Backend => _viewsBackendLazy.Value.Init(/*GetContext(),*/ Log);
        private ViewsExportImport ExportImport => _viewExportLazy.Value.Init(/*GetContext(),*/ Log);

        public ViewController(StatefulControllerDependencies dependencies, 
            Lazy<ViewsBackend> viewsBackendLazy,
            Lazy<ViewsExportImport> viewExportLazy) : base(dependencies)
        {
            _viewsBackendLazy = viewsBackendLazy;
            _viewExportLazy = viewExportLazy;
        }

        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "<Pending>")]
        public string Ping()
        {
            return "pong";
        }

        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public IEnumerable<ViewDetailsDto> All(int appId) => Backend.GetAll(appId);

        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public PolymorphismDto Polymorphism(int appId) => HttpContext.RequestServices.Build<PolymorphismBackend>().Init(Log).Polymorphism(appId);

        [HttpGet, HttpDelete]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public bool Delete(int appId, int id) => Backend.Delete(appId, id);

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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();

            if (HttpContext.Request.Form.Files.Count <= 0)
                return new ImportResultDto(false, "no file uploaded", Message.MessageTypes.Error);

            var files = HttpContext.Request.Form.Files;
            var streams = new List<FileUploadDto>();
            for (var i = 0; i < files.Count; i++)
                streams.Add(new FileUploadDto { Name = files[i].FileName, Stream = files[i].OpenReadStream() });
            var result = ExportImport.ImportView(zoneId, appId, streams, GetContext().Site.DefaultCultureCode);
            
            return wrapLog("ok", result);
        }
    }
}