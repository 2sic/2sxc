using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab)
    // we can't set this globally (only needed for imports)

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class AppPartsController : OqtStatefulControllerBase, IAppPartsController
    {
        private readonly Lazy<ExportContent> _exportContentLazy;
        private readonly Lazy<ImportContent> _importContentLazy;
        protected override string HistoryLogName => "Api.AParts";

        public AppPartsController(Lazy<ExportContent> exportContentLazy, Lazy<ImportContent> importContentLazy)
        {
            _exportContentLazy = exportContentLazy;
            _importContentLazy = importContentLazy;
        }

        #region Parts Export/Import

        /// <summary>
        /// Used to be GET ImportExport/GetContentInfo
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope)
            => _exportContentLazy.Value.Init(GetContext().Site.Id, GetContext().User, Log)
                .PreExportSummary(appId, zoneId, scope);

        /// <summary>
        /// Used to be GET ImportExport/ExportContent
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <param name="contentTypeIdsString"></param>
        /// <param name="entityIdsString"></param>
        /// <param name="templateIdsString"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Export(int zoneId, int appId, string contentTypeIdsString,
            string entityIdsString, string templateIdsString)
            => _exportContentLazy.Value.Init(GetContext().Site.Id, GetContext().User, Log)
                .Export(appId, zoneId, contentTypeIdsString, entityIdsString, templateIdsString);

        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            if (HttpContext.Request.Form.Files.Count <= 0) return new ImportResultDto(false, "no files uploaded");
            var file = HttpContext.Request.Form.Files[0];
            var result = _importContentLazy.Value.Init(GetContext().User, Log).Import(zoneId, appId, file.FileName,
                file.OpenReadStream(), GetContext().Site.DefaultCultureCode);

            return wrapLog("ok", result);
        }

        #endregion
    }
}