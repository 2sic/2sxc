using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [AllowAnonymous] // necessary at this level, because otherwise download would fail

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
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
            Lazy<ViewsExportImport> viewExportLazy) : base()
        {
            _viewsBackendLazy = viewsBackendLazy;
            _viewExportLazy = viewExportLazy;
        }

        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ViewDetailsDto> All(int appId) => Backend.GetAll(appId);

        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public PolymorphismDto Polymorphism(int appId) => HttpContext.RequestServices.Build<PolymorphismBackend>().Init(Log).Polymorphism(appId);

        [HttpGet, HttpDelete]
        //[SupportedModules("2sxc,2sxc-app")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
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
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
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

        ///// <summary>
        ///// Get usage statistics for entities so the UI can guide the user
        ///// to find out if data is being used or if it can be safely deleted.
        ///// </summary>
        ///// <param name="appId">App ID</param>
        ///// <param name="guid">Guid of the Entity</param>
        ///// <returns></returns>
        ///// <remarks>
        ///// New in 2sxc 11.11
        ///// </remarks>
        //[HttpGet]
        ////[SupportedModules("2sxc,2sxc-app")]
        ////[ValidateAntiForgeryToken]
        //[Authorize(Roles = RoleNames.Admin)]
        // TODO: implement Usage
        //public IEnumerable<ViewDto> Usage(int appId, Guid guid)
        //    => HttpContext.RequestServices.Build<UsageBackend>().Init(Log)
        //        .ViewUsage(appId, guid, (views, blocks) =>
        //        {
        //            // create array with all 2sxc modules in this portal
        //            var allMods = new Pages.Pages(Log).AllModulesWithContent(GetContext().Site.Id);
        //            Log.Add($"Found {allMods.Count} modules");

        //            return views.Select(vwb => new ViewDto().Init(vwb, blocks, allMods));
        //        });
    }
}