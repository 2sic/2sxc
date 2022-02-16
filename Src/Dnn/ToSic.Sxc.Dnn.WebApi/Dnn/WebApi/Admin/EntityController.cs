using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.ImportExport.Options;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;
using Guid = System.Guid;
using Helpers = ToSic.Eav.WebApi.Helpers;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Proxy Class to the EAV EntitiesController (Web API Controller)
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
	public class EntityController : SxcApiControllerBase, IEntitiesController
	{
        protected override string HistoryLogName => "Api.EntCnt";

        private IContextResolver ContextResolver
            => _contextResolver ?? (_contextResolver = GetService<IContextResolver>().Init(Log));
        private IContextResolver _contextResolver;

        /// <inheritdoc/>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType)
        {
            var appContext = ContextResolver.BlockOrApp(appId);
            return GetService<EntityApi>()
                .InitOrThrowBasedOnGrants(appContext, appContext.AppState, contentType,
                    GrantSets.ReadSomething, Log)
                .GetEntitiesForAdmin(contentType);
        }


        /// <inheritdoc/>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false, int? parentId = null, string parentField = null)
        {
            var appContext = ContextResolver.BlockOrApp(appId);
            GetService<EntityApi>()
                .InitOrThrowBasedOnGrants(appContext, appContext.AppState, contentType,
                    GrantSets.DeleteSomething, Log)
                .Delete(contentType, id, force, parentId, parentField);
        }

        /// <inheritdoc/>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false, int? parentId = null, string parentField = null)
        {
            var appContext = ContextResolver.BlockOrApp(appId);
            GetService<EntityApi>()
                .InitOrThrowBasedOnGrants(appContext, appContext.AppState, contentType,
                    GrantSets.DeleteSomething, Log)
                .Delete(contentType, guid, force, parentId, parentField);
        }


        /// <inheritdoc/>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
            => GetService<ContentExportApi>().Init(appId, Log).DownloadEntityAsJson(new DnnUser(), id, prefix, withMetadata);

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
        /// <remarks>
        /// This can't be in the interface, because the return-type is not .net core compatible. 
        /// </remarks>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Download(
            int appId,
            string language,
            string defaultLanguage,
            string contentType,
            ExportSelection recordExport, ExportResourceReferenceMode resourcesReferences,
            ExportLanguageResolution languageReferences, string selectedIds = null)
        {
            var fileContentAndFileName = GetService<ContentExportApi>().Init(appId, Log).ExportContent(
                new DnnUser(),
                language, defaultLanguage, contentType,
                recordExport, resourcesReferences,
                languageReferences, selectedIds);

            return Helpers.Download.BuildDownload(fileContentAndFileName.Item1, fileContentAndFileName.Item2);
        }

        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentImportResultDto XmlPreview(ContentImportArgsDto args)
            => GetService<ContentImportApi>().Init(args.AppId, Log).XmlPreview(args);


        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public ContentImportResultDto XmlUpload(ContentImportArgsDto args)
            => GetService<ContentImportApi>().Init(args.AppId, Log).XmlImport(args);

        /// <inheritdoc/>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Upload(EntityImportDto args) => GetService<ContentImportApi>().Init(args.AppId, Log).Import(args);


        /// <inheritdoc/>
        // not final yet, so no [HttpGet]
        public dynamic Usage(int appId, Guid guid) => GetService<EntityBackend>().Init(Log).Usage(appId, guid);

    }
}
