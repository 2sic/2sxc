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
    public class AppController : SxcApiControllerBase<AppControllerReal<HttpResponseMessage>>, IAppController<HttpResponseMessage>
    {
        public AppController() : base("App") { }

        protected override AppControllerReal<HttpResponseMessage> Real => base.Real.Init(PreventServerTimeout300);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<AppDto> List(int zoneId) => Real.List(zoneId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public List<AppDto> InheritableApps() => Real.InheritableApps();

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true) => Real.App(zoneId, appId, fullDelete);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, string name, int? inheritAppId = null) => Real.App(zoneId, name, inheritAppId);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<SiteLanguageDto> Languages(int appId) => Real.Languages(appId);

        /// <summary>
        /// Used to be GET ImportExport/GetAppInfo
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public AppExportInfoDto Statistics(int zoneId, int appId) => Real.Statistics(zoneId, appId);


        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool FlushCache(int zoneId, int appId) => Real.FlushCache(zoneId, appId);

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
        [ValidateAntiForgeryToken]
        public bool SaveData(int appId, int zoneId, bool includeContentGroups, bool resetAppGuid)
            => Real.SaveData(appId, zoneId, includeContentGroups, resetAppGuid);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public List<StackInfoDto> GetStack(int appId, string part, string key = null, Guid? view = null) 
            => Real.GetStack(appId, part, key, view);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Reset(int zoneId, int appId) 
            => Real.Reset(zoneId, appId, PortalSettings.DefaultLanguage);

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
            // TODO: ENHANCE WITH THE SAME OBJECT as used in ADAM upload (to determine request infos/uploaded files), and move to the real-implementation
            return request.Files.Count <= 0 
                ? new ImportResultDto(false, "no files uploaded") 
                : Real.Import(zoneId, request["Name"], request?.Files[0]?.InputStream);
        }
    }
}