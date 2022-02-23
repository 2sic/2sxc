using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Admin;
using AppDto = ToSic.Eav.WebApi.Dto.AppDto;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    [DnnLogExceptions]
    public class AppController : SxcApiControllerBase, IAppController
    {
        public AppController() : base("App") { }

        private AppControllerReal<HttpResponseMessage> RealController 
            => GetService<AppControllerReal<HttpResponseMessage>>().Init(PreventServerTimeout300, Log);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<AppDto> List(int zoneId)
            => RealController.List(zoneId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public List<AppDto> InheritableApps()
            => RealController.InheritableApps();

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true)
            => RealController.App(zoneId, appId, fullDelete);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, string name, int? inheritAppId = null)
            => RealController.App(zoneId, name, inheritAppId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<SiteLanguageDto> Languages(int appId) 
            => RealController.Languages(appId);

        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public AppExportInfoDto Statistics(int zoneId, int appId)
            => RealController.Statistics(zoneId, appId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool FlushCache(int zoneId, int appId)
            => RealController.FlushCache(zoneId, appId);

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
            => RealController.Export(appId, zoneId, includeContentGroups, resetAppGuid);

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
            => RealController.SaveData(appId, zoneId, includeContentGroups, resetAppGuid);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null) 
            => RealController.GetStack(appId, part, key, view);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Reset(int zoneId, int appId) 
            => RealController.Reset(zoneId, appId, PortalSettings.DefaultLanguage);

        /// <summary>
        /// Used to be POST ImportExport/ImportApp
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId)
        {
            var request = HttpContext.Current.Request;
            return request.Files.Count <= 0 
                ? new ImportResultDto(false, "no files uploaded") 
                : RealController.Import(zoneId, request["Name"], request?.Files[0]?.InputStream);
        }
    }
}