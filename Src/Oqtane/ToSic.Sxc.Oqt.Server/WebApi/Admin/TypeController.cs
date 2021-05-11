using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Assets;
using ToSic.Sxc.WebApi.ImportExport;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[SupportedModules("2sxc,2sxc-app")]
    //[DnnLogExceptions]
    [Authorize(Roles = RoleNames.Admin)]
    //[AutoValidateAntiforgeryToken]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/type/[action]")]

    [ValidateAntiForgeryToken]
    public class TypeController : OqtStatefulControllerBase, ITypeController
    {
        private readonly Lazy<ContentTypeApi> _ctApiLazy;
        private readonly Lazy<ContentExportApi> _contentExportLazy;
        protected override string HistoryLogName => "Api.Types";

        public TypeController(Lazy<ContentTypeApi> ctApiLazy, Lazy<ContentExportApi> contentExportLazy)
        {
            _ctApiLazy = ctApiLazy;
            _contentExportLazy = contentExportLazy;
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false)
            => _ctApiLazy.Value.Init(appId, Log).Get(scope, withStatistics);

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public IDictionary<string, string> Scopes(int appId)
            => State.Get(appId).ContentTypes.GetAllScopesWithLabels(); // new AppRuntime().Init(State.Identity(null, appId), false, Log).ContentTypes.ScopesWithLabels();

        /// <summary>
        /// Used to be GET ContentTypes/Scopes
        /// </summary>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => _ctApiLazy.Value.Init(appId, Log).GetSingle(contentTypeId, scope);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Delete(int appId, string staticName) => _ctApiLazy.Value.Init(appId, Log).Delete(staticName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
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
        [Authorize(Roles = RoleNames.Host)]
        public bool AddGhost(int appId, string sourceStaticName) => _ctApiLazy.Value.Init(appId, Log).CreateGhost(sourceStaticName);

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public void SetTitle(int appId, int contentTypeId, int attributeId)
            => _ctApiLazy.Value.Init(appId, Log).SetTitle(contentTypeId, attributeId);

        /// <summary>
        /// Used to be GET ContentExport/DownloadTypeAsJson
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, string name)
            => _contentExportLazy.Value.Init(appId, Log).DownloadTypeAsJson(GetContext().User, name);

        /// <summary>
        /// Used to be POST ImportExport/ImportContent
        /// </summary>
        /// <remarks>
        /// New in 2sxc 11.07
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]

        public ImportResultDto Import(int zoneId, int appId)
        {
            var wrapLog = Log.Call<ImportResultDto>();

            PreventServerTimeout300();
            if (HttpContext.Request.Form.Files.Count <= 0)
                return new ImportResultDto(false, "no file uploaded", Message.MessageTypes.Error);

            var files = HttpContext.Request.Form.Files;
            var streams = new List<FileUploadDto>();
            for (var i = 0; i < files.Count; i++)
                streams.Add(new FileUploadDto { Name = files[i].FileName, Stream = files[i].OpenReadStream() });
            var result = HttpContext.RequestServices.Build<ImportContent>().Init(GetContext().User, Log)
                .ImportContentType(zoneId, appId, streams, GetContext().Site.DefaultCultureCode);

            return wrapLog("ok", result);
        }
    }
}