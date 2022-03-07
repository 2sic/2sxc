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

        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<AppDto> List(int zoneId) => Real.List(zoneId);

        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public List<AppDto> InheritableApps() => Real.InheritableApps();

        /// <inheritdoc />
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, int appId, bool fullDelete = true) => Real.App(zoneId, appId, fullDelete);

        /// <inheritdoc />
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void App(int zoneId, string name, int? inheritAppId = null) => Real.App(zoneId, name, inheritAppId);

        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("2sxc,2sxc-app")]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public List<SiteLanguageDto> Languages(int appId) => Real.Languages(appId);

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public AppExportInfoDto Statistics(int zoneId, int appId) => Real.Statistics(zoneId, appId);


        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool FlushCache(int zoneId, int appId) => Real.FlushCache(zoneId, appId);

        /// <inheritdoc />
        [HttpGet]
        public HttpResponseMessage Export(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
            => Real.Export(zoneId, appId, includeContentGroups, resetAppGuid);

        /// <inheritdoc />
        [HttpGet]
        [ValidateAntiForgeryToken]
        public bool SaveData(int zoneId, int appId, bool includeContentGroups, bool resetAppGuid)
            => Real.SaveData(zoneId, appId, includeContentGroups, resetAppGuid);

        /// <inheritdoc />
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
        {
            PreventServerTimeout300();
            return Real.Reset(zoneId, appId, PortalSettings.DefaultLanguage);
        }

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId)
        {
            PreventServerTimeout300();
            var request = HttpContext.Current.Request;
            // TODO: @STV ENHANCE WITH THE SAME OBJECT as used in ADAM upload (to determine request infos/uploaded files), and move to the real-implementation
            return request.Files.Count <= 0 
                ? new ImportResultDto(false, "no files uploaded") 
                : Real.Import(zoneId, request["Name"], request?.Files[0]?.InputStream);
        }
    }
}