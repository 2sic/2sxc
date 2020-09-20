using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;
using Guid = System.Guid;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
	public partial class EntityController : SxcApiControllerBase, IEntitiesController
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
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> List(int appId, string contentType) 
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.ReadSomething, Log)
                .GetEntitiesForAdmin(contentType);


        [HttpDelete]
        // todo: unsure why only Edit - is this used anywhere else than admin?
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false) 
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, id, force);

        [HttpDelete]
        // todo: unsure why only Edit - is this used anywhere else than admin?
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
            => EntityApi.GetOrThrowBasedOnGrants(GetContext(), GetApp(appId), contentType, GrantSets.DeleteSomething, Log)
                .Delete(contentType, guid, force);


        // New feature in 11.03 - Usage Statistics
        // not final yet, so no [HttpGet]
        public dynamic Usage(int appId, Guid guid) => new EntityBackend().Init(Log).Usage(GetContext(), GetApp(appId), guid);

    }
}
