using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.AppStack;
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
    [Route(WebApiConstants.WebApiStateRoot + "/admin/app/[action]")]
    public class AppController : OqtStatefulControllerBase, IAppController
    {
        private readonly Lazy<AppsBackend> _appsBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        private readonly Lazy<ExportApp> _exportAppLazy;
        private readonly Lazy<ImportApp> _importAppLazy;
        private readonly Lazy<AppCreator> _appBuilderLazy;
        private readonly Lazy<ResetApp> _resetAppLazy;
        private readonly Lazy<SystemManager> _systemManagerLazy;
        private AppControllerReal RealController { get; }
        protected override string HistoryLogName => "Api.App";

        public AppController(
            Lazy<AppsBackend> appsBackendLazy,
            Lazy<CmsZones> cmsZonesLazy,
            Lazy<ExportApp> exportAppLazy,
            Lazy<ImportApp> importAppLazy,
            Lazy<AppCreator> appBuilderLazy,
            Lazy<ResetApp> resetAppLazy,
            Lazy<SystemManager> systemManagerLazy,
            AppControllerReal appControllerReal)
        {
            _appsBackendLazy = appsBackendLazy;
            _cmsZonesLazy = cmsZonesLazy;
            _exportAppLazy = exportAppLazy;
            _importAppLazy = importAppLazy;
            _appBuilderLazy = appBuilderLazy;
            _resetAppLazy = resetAppLazy;
            _systemManagerLazy = systemManagerLazy;
            RealController = appControllerReal.Init(Log);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<AppDto> List(int zoneId)
            => _appsBackendLazy.Value.Init(Log).Apps();

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Host)]
        public List<AppDto> InheritableApps()
            => _appsBackendLazy.Value.Init(Log).GetInheritableApps();


        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true)
            => _cmsZonesLazy.Value.Init(zoneId, Log).AppsMan.RemoveAppInSiteAndEav(appId, fullDelete);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int zoneId, string name, int? inheritAppId = null)
            => _appBuilderLazy.Value.Init(zoneId, Log).Create(name, null, inheritAppId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<SiteLanguageDto> Languages(int appId) => RealController.Languages(appId);

        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public AppExportInfoDto Statistics(int zoneId, int appId)
            => _exportAppLazy.Value.Init(Log).GetAppInfo(appId, zoneId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public bool FlushCache(int zoneId, int appId)
        {
            var wrapLog = Log.Call<bool>($"{zoneId}, {appId}");
            _systemManagerLazy.Value.Init(Log).Purge(zoneId, appId);
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
        public IActionResult Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid) =>
            _exportAppLazy.Value.Init(Log)
                .Export(appId, zoneId, includeContentGroups, resetAppGuid);

        /// <inheritdoc />
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Reset(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            var result = _resetAppLazy.Value.Init(Log).Reset(zoneId, appId, GetContext().Site.DefaultCultureCode);

            return wrapLog("ok", result);
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null) =>
            ServiceProvider.Build<AppStackBackend>()
                .GetAll(appId, part ?? AppConstants.RootNameSettings, key, view, null);


        /// <summary>
        /// Used to be GET ImportExport/ExportForVersionControl
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        [ValidateAntiForgeryToken]
        public bool SaveData(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => _exportAppLazy.Value.Init(Log).SaveDataForVersionControl(appId, zoneId, includeContentGroups, resetAppGuid);


        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ImportResultDto Import(int zoneId)
        {
            Log.Add("import app start");

            // Ensure that Hot Reload is not enabled or try to disable it.
            HotReloadEnabledCheck.Check();

            var request = HttpContext.Request.Form;

            PreventServerTimeout300();

            if (request.Files.Count <= 0) return new ImportResultDto(false, "no files uploaded");

            return _importAppLazy.Value.Init(Log)
                .Import(zoneId, request["Name"], request.Files[0].OpenReadStream());
        }
    }
}