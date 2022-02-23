using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    public class AppPartsController : DnnApiControllerWithFixes, IAppPartsController
    {
        public AppPartsController() : base("Parts") { }

        #region Parts Export/Import

        /// <summary>
        /// Used to be GET ImportExport/GetContentInfo
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="appId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope)
            => GetService<ExportContent>().Init(Log).PreExportSummary(appId, zoneId, scope);

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
            => GetService<ExportContent>().Init(Log)
                .Export(appId, zoneId, contentTypeIdsString, entityIdsString, templateIdsString);


        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            if (HttpContext.Current.Request.Files.Count <= 0) return new ImportResultDto(false, "no files uploaded");
            var file = HttpContext.Current.Request.Files[0];
            var result = GetService<ImportContent>().Init(new DnnUser(), Log).Import(zoneId, appId, file.FileName,
                file.InputStream, PortalSettings.DefaultLanguage);

            return wrapLog("ok", result);
        }

        #endregion


    }
}