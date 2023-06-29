using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using Guid = System.Guid;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the EAV EntitiesController (Web API Controller)
    /// </summary>
    /// <remarks>
    /// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
    /// So we can't use the classic protection attributes to the class like:
    /// - [SupportedModules(DnnSupportedModuleNames)]
    /// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    /// - [ValidateAntiForgeryToken]
    /// Instead, each method must have all attributes, or do additional security checking.
    /// Security checking is possible, because the cookie still contains user information
    /// </remarks>
    [DnnLogExceptions]
	public class EntityController : SxcApiControllerBase<EntityControllerReal<HttpResponseMessage>>, IEntityController<HttpResponseMessage>
	{
        public EntityController(): base(EntityControllerReal<HttpResponseMessage>.LogSuffix) { }


        /// <inheritdoc/>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType) => SysHlp.Real.List(appId, contentType);


        /// <inheritdoc/>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int appId, int? id = null, Guid? guid = null, bool force = false, int? parentId = null, string parentField = null)
        => SysHlp.Real.Delete(contentType, appId, id, guid, force, parentId, parentField);


        /// <inheritdoc/>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = SysHlp.GetResponseMaker();
            responseMaker.Init(this);

            return SysHlp.Real.Json(appId, id, prefix, withMetadata);
        }


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
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = SysHlp.GetResponseMaker();
            responseMaker.Init(this);

            return SysHlp.Real.Download(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
                languageReferences, selectedIds);
        }


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentImportResultDto XmlPreview(ContentImportArgsDto args) => SysHlp.Real.XmlPreview(args);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentImportResultDto XmlUpload(ContentImportArgsDto args) => SysHlp.Real.XmlUpload(args);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Upload(EntityImportDto args) => SysHlp.Real.Upload(args);


        ///// <inheritdoc/>
        //// not final yet, so no [HttpGet]
        //public dynamic Usage(int appId, Guid guid) => Real.Usage(appId, guid);
    }
}
