using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;
using System.Net.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.ImportExport;
using AppDto = ToSic.Sxc.WebApi.App.AppDto;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    [Route(WebApiConstants.WebApiStateRoot + "/admin/app/[action]")]
    public class AppController : SxcStatefulControllerBase, IAppController
    {
        protected override string HistoryLogName => "Api.App";

        public AppController(StatefulControllerDependencies dependencies) : base(dependencies)
        { }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public List<AppDto> List(int zoneId)
            => new AppsBackend().Init(Log).Apps(GetContext().Tenant, GetBlock(), zoneId);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public void App(int zoneId, int appId)
            => new CmsZones(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public void App(int zoneId, string name)
            => AppManager.AddBrandNewApp(zoneId, name, Log);


        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public AppExportInfoDto Statistics(int zoneId, int appId)
            => Eav.Factory.Resolve<ExportApp>().Init(GetContext().Tenant.Id, GetContext().User, Log)
                .GetAppInfo(appId, zoneId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            SystemManager.Purge(zoneId, appId, log: Log);
            return wrapLog("ok", true);
        }

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
            => Eav.Factory.Resolve<ExportApp>().Init(GetContext().Tenant.Id, GetContext().User, Log)
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
            => Eav.Factory.Resolve<ExportApp>().Init(GetContext().Tenant.Id, GetContext().User, Log)
                .SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);


        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public ImportResultDto Import(int zoneId)
        {
            Log.Add("import app start");

            var request = HttpContext.Request.Form;

            PreventServerTimeout300();

            if (request.Files.Count <= 0) return new ImportResultDto(false, "no files uploaded");

            return Eav.Factory.Resolve<ImportApp>().Init(GetContext().User, Log)
                .Import(zoneId, request["Name"], request.Files[0].OpenReadStream());
        }
    }
}