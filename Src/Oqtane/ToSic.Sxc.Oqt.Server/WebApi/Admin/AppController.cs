using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab)
    // we can't set this globally (only needed for imports)

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + "/admin/app/[action]")]

    public class AppController : OqtStatefulControllerBase<AppControllerReal<IActionResult>>, IAppController<IActionResult>
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public AppController(): base("App") { }

        protected override AppControllerReal<IActionResult> Real => base.Real.Init(PreventServerTimeout300);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<AppDto> List(int zoneId)
            => Real.List(zoneId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Host)]
        public List<AppDto> InheritableApps()
            => Real.InheritableApps();


        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true)
            => Real.App(zoneId, appId, fullDelete);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public void App(int zoneId, string name, int? inheritAppId = null)
            => Real.App(zoneId, name, inheritAppId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<SiteLanguageDto> Languages(int appId) 
            => Real.Languages(appId);

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
            => Real.Statistics( zoneId, appId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public bool FlushCache(int zoneId, int appId)
            => Real.FlushCache(zoneId, appId);

        /// <summary>
        /// Used to be GET ImportExport/ExportApp
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <param name="includeContentGroups"></param>
        /// <param name="resetAppGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Export(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid) 
            => Real.Export(appId, zoneId, includeContentGroups, resetAppGuid);

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
            => Real.SaveData(appId, zoneId, includeContentGroups, resetAppGuid);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null)
            => Real.GetStack(appId, part, key, view);

        /// <inheritdoc />
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Reset(int zoneId, int appId) 
            => Real.Reset(zoneId, appId, GetContext().Site.DefaultCultureCode);

        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ImportResultDto Import(int zoneId)
        {
            // Ensure that Hot Reload is not enabled or try to disable it.
            HotReloadEnabledCheck.Check();
            var request = HttpContext.Request.Form;
            return request.Files.Count <= 0 
                ? new ImportResultDto(false, "no files uploaded") 
                : Real.Import(zoneId, request["Name"], request.Files[0].OpenReadStream());
        }
    }
}