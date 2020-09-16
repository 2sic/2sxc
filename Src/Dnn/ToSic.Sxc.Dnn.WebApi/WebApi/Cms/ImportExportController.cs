using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.WebApi.Cms
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    public class ImportExportController : DnnApiControllerWithFixes, IImportExportController
    {
        protected override string HistoryLogName => "Api.2sIExC";

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public AppExportInfoDto GetAppInfo(int appId, int zoneId)
            => Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .GetAppInfo(appId, zoneId);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ExportPartsOverviewDto GetContentInfo(int appId, int zoneId, string scope)
            => Factory.Resolve<ExportContent>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .PreExportSummary(appId, zoneId, scope);


        [HttpGet]
        public HttpResponseMessage ExportApp(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .Export(appId, zoneId, includeContentGroups, resetAppGuid);

        [HttpGet]
        [ValidateAntiForgeryToken]
        public bool ExportForVersionControl(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => Factory.Resolve<ExportApp>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);

        [HttpGet]
        public HttpResponseMessage ExportContent(int appId, int zoneId, string contentTypeIdsString,
            string entityIdsString, string templateIdsString)
            => Factory.Resolve<ExportContent>().Init(PortalSettings.PortalId, new DnnUser(UserInfo), Log)
                .Export(appId, zoneId, contentTypeIdsString, entityIdsString, templateIdsString);


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto ImportApp()
        {
            Log.Add("import app start");

            var request = HttpContext.Current.Request;
            // Increase script timeout to prevent timeouts
            HttpContext.Current.Server.ScriptTimeout = 300;

            var zoneId  = int.Parse(request["ZoneId"]);
            if (request.Files.Count <= 0) return new ImportResultDto();

            return Factory.Resolve<ImportApp>().Init(new DnnUser(UserInfo), Log)
                .Import(zoneId, request["Name"], request.Files[0].InputStream, Exceptions.LogException);
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto ImportContent()
        {
            var wrapLog = Log.Call<ImportResultDto>();

            var request = HttpContext.Current.Request;
            var appId = int.Parse(request["AppId"]);
            var zoneId = int.Parse(request["ZoneId"]);

            if (request.Files.Count <= 0) return new ImportResultDto();

            HttpContext.Current.Server.ScriptTimeout = 300;

            var result = Factory.Resolve<ImportContent>().Init(new DnnUser(UserInfo), Log).Import(zoneId, appId, request.Files[0].FileName,
                request.Files[0].InputStream, PortalSettings.DefaultLanguage, Exceptions.LogException);

            return wrapLog("ok", result);
        }

    }
}