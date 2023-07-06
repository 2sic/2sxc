using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using RealController = ToSic.Sxc.WebApi.Admin.AppPartsControllerReal<System.Net.Http.HttpResponseMessage>;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] can't be used, because it forces the security
    // token, which fails in the cases where the url is called using get, which should result in a download

    public class AppPartsController : DnnApiControllerWithFixes, IAppPartsController<HttpResponseMessage>
    {
        public AppPartsController() : base(RealController.LogSuffix) { }
        private RealController Real => SysHlp.GetService<RealController>();
        #region Parts Export/Import

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ExportPartsOverviewDto Get(int zoneId, int appId, string scope) => Real.Get(zoneId: zoneId, appId: appId, scope: scope);


        /// <inheritdoc />
        [HttpGet]
        public HttpResponseMessage Export(int zoneId, int appId, string contentTypeIdsString, string entityIdsString, string templateIdsString)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = SysHlp.GetResponseMaker();
            responseMaker.Init(this);

            return Real.Export(zoneId: zoneId, appId: appId, contentTypeIdsString: contentTypeIdsString,
                entityIdsString: entityIdsString, templateIdsString: templateIdsString);
        }


        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId)
        {
            SysHlp.PreventServerTimeout300();
            return Real.Import(uploadInfo: new HttpUploadedFile(Request, HttpContext.Current.Request), zoneId: zoneId, appId: appId);
        }

        #endregion




    }
}