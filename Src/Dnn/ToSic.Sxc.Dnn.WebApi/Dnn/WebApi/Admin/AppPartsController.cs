using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download
    // [ValidateAntiForgeryToken] because the exports are called by the browser directly (new tab) 
    // we can't set this globally (only needed for imports)
    public class AppPartsController : DnnApiControllerWithFixes<AppPartsControllerReal>, IAppPartsController
    {
        public AppPartsController() : base(AppPartsControllerReal.LogSuffix) { }

        #region Parts Export/Import

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => Real.Get(appId, zoneId, scope);


        /// <inheritdoc />
        [HttpGet]
        public HttpResponseMessage Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
            => Real.Export(appId, zoneId, contentTypeIdsString, entityIdsString, templateIdsString);


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId)
        {
            PreventServerTimeout300();
            return Real.Import(new HttpUploadedFile(Request, HttpContext.Current.Request), zoneId, appId);
        }

        #endregion




    }
}