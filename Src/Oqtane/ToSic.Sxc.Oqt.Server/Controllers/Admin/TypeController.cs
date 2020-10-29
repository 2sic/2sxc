using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //   [DnnLogExceptions]
    //   [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    //[AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/type/[action]")]
    [Route("{alias}/api/[controller]")]
    //[ApiController]

    //[ValidateAntiForgeryToken]
    public class TypeController : SxcStatefulControllerBase, ITypeController
    {
        private readonly OqtaneContextBuilder _contextBuilder;
        protected override string HistoryLogName => "Api.Types";
        private ContentTypeApi Backend => new ContentTypeApi(Log);
        public TypeController(StatefulControllerDependencies dependencies, OqtaneContextBuilder contextBuilder) : base(dependencies)
        {
            _contextBuilder = contextBuilder;
        }

        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3400:Methods should not return constants", Justification = "<Pending>")]
        public string Ping()
        {
            return "pong";
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false)
            => Backend.Get(appId, scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IDictionary<string, string> Scopes(int appId)
            => new AppRuntime(appId, false, Log).ContentTypes.ScopesWithLabels();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Backend.GetSingle(appId, contentTypeId, scope);

        [HttpDelete]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public bool Delete(int appId, string staticName) => Backend.Delete(appId, staticName);

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
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
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool AddGhost(int appId, string sourceStaticName) => Backend.CreateGhost(appId, sourceStaticName);

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
            => Backend.SetTitle(appId, contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentExport/DownloadTypeAsJson
        /// </summary>
        [HttpGet]
        //[AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, string name)
            => new Eav.WebApi.ContentExportApi(Log).DownloadTypeAsJson(GetContext().User, appId, name);
    }
}