using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;

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
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    public class EntityController : OqtStatefulControllerBase<EntityControllerReal>, IEntityController
    {
        public EntityController() : base(EntityControllerReal.LogSuffix) { }


        /// <inheritdoc/>
        [HttpGet]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType) => Real.List(appId, contentType);


        /// <inheritdoc/>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void Delete(
            [FromQuery] string contentType,
            [FromQuery] int? id, 
            [FromQuery] Guid? guid, 
            [FromQuery] int appId, 
            [FromQuery] bool force = false,
            [FromQuery] int? parentId = null, 
            [FromQuery] string parentField = null) =>
            Real.Delete(contentType, id, guid, appId, force, parentId, parentField);


        /// <inheritdoc/>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata) => Real.Json(appId, id, prefix, withMetadata);


        /// <inheritdoc/>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Download(
            int appId,
            string language,
            string defaultLanguage,
            string contentType,
            ExportSelection recordExport, 
            ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, 
            string selectedIds = null) 
            => Real.Download(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences, languageReferences, selectedIds);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public ContentImportResultDto XmlPreview(ContentImportArgsDto args) => Real.XmlPreview(args);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public ContentImportResultDto XmlUpload(ContentImportArgsDto args) => Real.XmlUpload(args);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Upload(EntityImportDto args) => Real.Upload(args);


        //// New feature in 11.03 - Usage Statistics
        //// not final yet, so no [HttpGet]
        //public dynamic Usage(int appId, Guid guid) => Real.Usage(appId, guid);
    }
}