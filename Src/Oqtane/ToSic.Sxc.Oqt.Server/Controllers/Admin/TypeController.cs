using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //   [DnnLogExceptions]
    [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
    [AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/type/[action]")]
    //[ApiController]
    [ValidateAntiForgeryToken]
    public class TypeController : OqtStatefulControllerBase, ITypeController
    {
        private readonly Lazy<ContentTypeApi> _ctApiLazy;
        private readonly Lazy<ContentExportApi> _contentExportLazy;
        protected override string HistoryLogName => "Api.Types";

        public TypeController(StatefulControllerDependencies dependencies,
            Lazy<ContentTypeApi> ctApiLazy,
            Lazy<ContentExportApi> contentExportLazy
            ) : base(dependencies)
        {
            _ctApiLazy = ctApiLazy;
            _contentExportLazy = contentExportLazy;
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false)
            => _ctApiLazy.Value.Init(appId, Log).Get(scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public IDictionary<string, string> Scopes(int appId)
            => State.Get(appId).ContentTypes.GetAllScopesWithLabels(); // new AppRuntime().Init(State.Identity(null, appId), false, Log).ContentTypes.ScopesWithLabels();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => _ctApiLazy.Value.Init(appId, Log).GetSingle(contentTypeId, scope);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public bool Delete(int appId, string staticName) => _ctApiLazy.Value.Init(appId, Log).Delete(staticName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
        // it causes problems when a content-type has metadata, where a value then is a deeper object
        // in future, the JS front-end should send something clearer and not the whole object
        public bool Save(int appId, [FromBody] Dictionary<string, object> item)
        {
            var cleanList = item.ToDictionary(i => i.Key, i => i.Value?.ToString());
            return _ctApiLazy.Value.Init(appId, Log).Save(cleanList);
        }

        /// <summary>
        /// Used to be GET ContentType/CreateGhost
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="sourceStaticName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.HostRole)]
        public bool AddGhost(int appId, string sourceStaticName) => _ctApiLazy.Value.Init(appId, Log).CreateGhost(sourceStaticName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
            => _ctApiLazy.Value.Init(appId, Log).SetTitle(contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentExport/DownloadTypeAsJson
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, string name)
            => _contentExportLazy.Value.Init(appId, Log).DownloadTypeAsJson(GetContext().User, name);
    }
}