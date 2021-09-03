using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the EAV EntitiesController (Web API Controller)
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
    //[DnnLogExceptions]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/entity/[action]")]
    public class EntityController : OqtStatefulControllerBase //, IEntitiesController // FIX: changed from interface to solve ambiguous DELETE routes.
    {
        private readonly Lazy<ContentExportApi>_contentExportLazy;
        private readonly Lazy<ContentImportApi> _contentImportLazy;
        private readonly Lazy<EntityApi> _lazyEntityApi;
        protected override string HistoryLogName => "Api.EntCnt";
        
        public EntityController(Lazy<EntityApi> lazyEntityApi, Lazy<ContentExportApi> contentExportLazy, Lazy<ContentImportApi> contentImportLazy)
        {
            _contentExportLazy = contentExportLazy;
            _contentImportLazy = contentImportLazy;
            _lazyEntityApi = lazyEntityApi;
        }

        /// <summary>
        /// Used to be Entities/GetOllOfTypeForAdmin
        /// Used to be Entities/GetEntities
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType)
            => _lazyEntityApi.Value.InitOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.ReadSomething, Log)
                .GetEntitiesForAdmin(contentType);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void Delete([FromQuery] string contentType, [FromQuery] int? id, [FromQuery] Guid? guid, [FromQuery] int appId, [FromQuery] bool force = false)
        {
            if (id.HasValue) _lazyEntityApi.Value.InitOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, id.Value, force);
            else if (guid.HasValue) _lazyEntityApi.Value.InitOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, guid.Value, force);
        }

        /// <summary>
        /// Used to be GET ContentExport/DownloadEntityAsJson
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
            => _contentExportLazy.Value.Init(appId, Log).DownloadEntityAsJson(GetContext().User, id, prefix, withMetadata);


        /// <summary>
        /// Used to be GET ContentExport/ExportContent
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="language"></param>
        /// <param name="defaultLanguage"></param>
        /// <param name="contentType"></param>
        /// <param name="recordExport"></param>
        /// <param name="resourcesReferences"></param>
        /// <param name="languageReferences"></param>
        /// <param name="selectedIds"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public async Task<IActionResult> Download(
            int appId,
            string language,
            string defaultLanguage,
            string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            var fileContentAndFileName = _contentExportLazy.Value.Init(appId, Log).ExportContent(
                GetContext().User,
                language, defaultLanguage, contentType,
                recordExport, resourcesReferences,
                languageReferences, selectedIds);

            var fileContents = Encoding.Unicode.GetBytes(fileContentAndFileName.Item1);
            var fileName = fileContentAndFileName.Item2;

            return File(fileContents: fileContents, contentType: MediaTypeNames.Application.Octet, fileDownloadName: fileName);
        }

        /// <summary>
        /// This seems to be for XML import of a list
        /// Used to be POST ContentImport/EvaluateContent
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public ContentImportResultDto XmlPreview(ContentImportArgsDto args)
            => _contentImportLazy.Value.Init(args.AppId, Log).XmlPreview(args);

        /// <summary>
        /// This seems to be for XML import of a list
        /// Used to be POST ContentImport/ImportContent
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public ContentImportResultDto XmlUpload(ContentImportArgsDto args)
            => _contentImportLazy.Value.Init(args.AppId, Log).XmlImport(args);

        /// <summary>
        /// This is the single-item json import
        /// Used to be POST ContentImport/Import
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Upload(EntityImportDto args) => _contentImportLazy.Value.Init(args.AppId, Log).Import(args);

        // New feature in 11.03 - Usage Statistics
        // not final yet, so no [HttpGet]
        //public dynamic Usage(int appId, Guid guid) => _lazyEntityBackend.Value.Init(Log).Usage(appId, guid);
    }
}