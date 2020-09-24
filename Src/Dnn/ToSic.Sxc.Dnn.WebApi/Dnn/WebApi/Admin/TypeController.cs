using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.ImportExport;
using ContentTypeApi = ToSic.Eav.WebApi.ContentTypeApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    /// <remarks>
    /// Because the JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
    /// So we can't use the classic protection attributes like:
    /// - [SupportedModules("2sxc,2sxc-app")]
    /// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    /// - [ValidateAntiForgeryToken]
    /// Instead, the method itself must do additional security checking.
    /// Security checking is possible, because the cookie still contains user information
    /// </remarks>
    [AllowAnonymous]
    [DnnLogExceptions]
    public class TypeController : SxcApiControllerBase, ITypeController
    {
        protected override string HistoryLogName => "Api.Types";

        private ContentTypeApi Backend => new ContentTypeApi(Log);

        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false) 
            => Backend.Get(appId, scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IDictionary<string, string> Scopes(int appId) 
            => new AppRuntime(appId, false, Log).ContentTypes.ScopesWithLabels();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Backend.GetSingle(appId, contentTypeId, scope);


        [HttpDelete]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) => Backend.Delete(appId, staticName);

	    [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return Backend.Save(appId, cleanList);
        }

        /// <summary>
        /// Used to be GET ContentType/CreateGhost
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="sourceStaticName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool AddGhost(int appId, string sourceStaticName) => Backend.CreateGhost(appId, sourceStaticName);



	    [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId) 
            => Backend.SetTitle(appId, contentTypeId, attributeId);


        /// <summary>
        /// Used to be GET ContentExport/DownloadTypeAsJson
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, string name)
            => new Eav.WebApi.ContentExportApi(Log).DownloadTypeAsJson(new DnnUser(UserInfo), appId, name);



        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            if (HttpContext.Current.Request.Files.Count <= 0)
                return new ImportResultDto(false, "no file uploaded", Message.MessageTypes.Error);
            
            var file = HttpContext.Current.Request.Files[0];
            var result = Eav.Factory.Resolve<ImportContent>().Init(new DnnUser(UserInfo), Log).ImportContentType(zoneId, appId, 
                file.InputStream, PortalSettings.DefaultLanguage);

            return wrapLog("ok", result);
        }

    }
}