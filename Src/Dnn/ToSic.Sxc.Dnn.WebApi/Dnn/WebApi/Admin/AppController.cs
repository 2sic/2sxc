using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.ImportExport;
using AppDto = ToSic.Sxc.WebApi.App.AppDto;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    [DnnLogExceptions]
    public class AppController : SxcApiControllerBase, IAppController
    {
        protected override string HistoryLogName => "Api.App";

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<AppDto> List(int zoneId)
            => new AppsBackend().Init(Log).Apps(new DnnTenant().Init(ActiveModule.OwnerPortalID), GetBlock(), zoneId);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, int appId)
            => new CmsZones(zoneId, Log).AppsMan.RemoveAppInTenantAndEav(appId);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, string name)
            => AppManager.AddBrandNewApp(zoneId, name, Log);


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