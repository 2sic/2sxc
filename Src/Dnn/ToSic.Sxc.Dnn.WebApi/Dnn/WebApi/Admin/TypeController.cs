using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.ImportExport;
using ContentTypeApi = ToSic.Eav.WebApi.ContentTypeApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Web API Controller for Content-Type structures, fields etc.
    /// </summary>
    /// <remarks>
    /// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
    /// So we can't use the classic protection attributes to the class like:
    /// - [SupportedModules("2sxc,2sxc-app")]
    /// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    /// - [ValidateAntiForgeryToken]
    /// Instead, each method must have all attributes, or do additional security checking.
    /// Security checking is possible, because the cookie still contains user information
    /// </remarks>
    [DnnLogExceptions]
    public class TypeController : SxcApiControllerBase, ITypeController
    {
        /// <summary>
        /// Name of this class in the insights logs.
        /// </summary>
        protected override string HistoryLogName => "Api.Types";

        private ContentTypeApi Backend => GetService<ContentTypeApi>();

        /// <summary>
        /// Get a list of all content-types.
        /// See https://docs.2sxc.org/basics/data/content-types/index.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="scope"></param>
        /// <param name="withStatistics"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false) 
            => Backend.Init(appId, Log).Get(scope, withStatistics);

        /// <summary>
        /// Used to be GET Scopes.
        /// Scopes are a way to organize content types, see https://docs.2sxc.org/basics/data/content-types/scopes.html
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IDictionary<string, string> Scopes(int appId)
            => Backend.Init(appId, Log).Scopes();

        /// <summary>
        /// Used to be GET ContentTypes.
        /// See https://docs.2sxc.org/basics/data/content-types/index.html
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Backend.Init(appId, Log).GetSingle(contentTypeId, scope);

        /// <summary>
        /// Delete a Content-Type
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="staticName"></param>
        /// <returns></returns>
        /// <remarks>
        /// ATM it requires the DELETE verb, but this often causes problems on IIS with WebDav.
        /// TODO: probably switch over to use Get again, even if it's not as descriptive as delete
        /// </remarks>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) => Backend.Init(appId, Log).Delete(staticName);

        /// <summary>
        /// Save a Content-Type.
        /// See https://docs.2sxc.org/basics/data/content-types/index.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
	    [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return Backend.Init(appId, Log).Save(cleanList);
        }

        /// <summary>
        /// Used to add a Ghost content-type.
        /// See https://docs.2sxc.org/basics/data/content-types/range-app-shared.html
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="sourceStaticName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool AddGhost(int appId, string sourceStaticName) => Backend.Init(appId, Log).CreateGhost(sourceStaticName);


        /// <summary>
        /// Change which attribute on a Content-Type is the title. 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contentTypeId"></param>
        /// <param name="attributeId"></param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
            => Backend.Init(appId, Log).SetTitle(contentTypeId, attributeId);


        /// <summary>
        /// Export a Content-Type as JSON
        /// </summary>
        /// <remarks>
        /// New in 2sxc 11.07
        /// </remarks>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, string name)
            => GetService<ContentExportApi>().Init(appId, Log).DownloadTypeAsJson(new DnnUser(), name);



        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <remarks>
        /// New in 2sxc 11.07
        /// </remarks>
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

            var files = HttpContext.Current.Request.Files;
            var streams = new List<FileUploadDto>();
            for(var i = 0; i < files.Count; i++)
                streams.Add(new FileUploadDto { Name = files[i].FileName, Stream = files[i].InputStream});
            var result = GetService<ImportContent>().Init(new DnnUser(), Log)
                .ImportContentType(zoneId, appId, streams, PortalSettings.DefaultLanguage);

            return wrapLog("ok", result);
        }
    }
}