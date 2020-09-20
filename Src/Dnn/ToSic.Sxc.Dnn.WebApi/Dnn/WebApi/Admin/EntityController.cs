using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;
using Guid = System.Guid;

namespace ToSic.Sxc.Dnn.WebApi.Admin
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
    [AllowAnonymous]
    [DnnLogExceptions]
	public class EntityController : SxcApiControllerBase, IEntitiesController
	{
        protected override string HistoryLogName => "Api.EntCnt";

        /// <summary>
        /// Used to be Entities/GetOllOfTypeForAdmin
        /// Used to be Entities/GetEntities
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType) 
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.ReadSomething, Log)
                .GetEntitiesForAdmin(contentType);


        [HttpDelete]
        [ValidateAntiForgeryToken]
        // todo: unsure why only Edit - is this used anywhere else than admin?
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false) 
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, id, force);

        [HttpDelete]
        [ValidateAntiForgeryToken]
        // todo: unsure why only Edit - is this used anywhere else than admin?
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, guid, force);


        /// <summary>
        /// Used to be GET ContentExport/DownloadEntityAsJson
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
            => new ContentExportApi(Log).DownloadEntityAsJson(new DnnUser(UserInfo), appId, id, prefix, withMetadata);


        // New feature in 11.03 - Usage Statistics
        // not final yet, so no [HttpGet]
        public dynamic Usage(int appId, Guid guid) => new EntityBackend().Init(Log).Usage(GetContext(), GetApp(appId), guid);

    }
}
