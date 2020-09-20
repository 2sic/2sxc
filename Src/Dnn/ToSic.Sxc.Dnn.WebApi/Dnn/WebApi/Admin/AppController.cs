using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    public class AppController : DnnApiControllerWithFixes, IAppController
    {
        protected override string HistoryLogName => "Api.App";

        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public AppExportInfoDto Statistics(int appId, int zoneId)
            => Eav.Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .GetAppInfo(appId, zoneId);

        /// <summary>
        /// Used to be GET ImportExport/ExportApp
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => Eav.Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .Export(appId, zoneId, includeContentGroups, resetAppGuid);

        /// <summary>
        /// Used to be GET ImportExport/ExportForVersionControl
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        public bool SaveData(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => Eav.Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);


        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId)
        {
            Log.Add("import app start");

            var request = HttpContext.Current.Request;
            PreventServerTimeout300();

            if (request.Files.Count <= 0) return new ImportResultDto();

            return Eav.Factory.Resolve<ImportApp>().Init(new DnnUser(UserInfo), Log)
                .Import(zoneId, request["Name"], request.Files[0].InputStream, Exceptions.LogException);
        }


    }
}