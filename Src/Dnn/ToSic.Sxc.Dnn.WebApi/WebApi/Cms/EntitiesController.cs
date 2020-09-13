using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Security;
using Guid = System.Guid;

namespace ToSic.Sxc.WebApi.Cms
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
	public partial class EntitiesController : SxcApiControllerBase, IEntitiesController
	{
        protected override string HistoryLogName => "Api.EntCnt";

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        /// <remarks>
        /// Is currently only used in Queries / Pipelines to list the items
        /// TODO: should be merged with the GetAllOfTypeForAdmin
        /// </remarks>
        [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId)
	        => new EntityApi(appId, true, Log).GetEntities(contentType);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType) 
            => GetApiOrThrowIfMissingPermissions(appId, contentType, GrantSets.ReadSomething)
                .GetEntitiesForAdmin(contentType);


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false) 
            => GetApiOrThrowIfMissingPermissions(appId, contentType, GrantSets.DeleteSomething)
                .Delete(contentType, id, force);

        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false) 
            => GetApiOrThrowIfMissingPermissions(appId, contentType, GrantSets.DeleteSomething)
                .Delete(contentType, guid, force);

        private EntityApi GetApiOrThrowIfMissingPermissions(int appId, string contentType, List<Eav.Security.Grants> requiredGrants)
        {
            var permCheck = new MultiPermissionsTypes().Init(GetContext(), GetApp(appId), contentType, Log);
            if (!permCheck.EnsureAll(requiredGrants, out var error))
                throw HttpException.PermissionDenied(error);
            return new EntityApi(appId, true, Log);
        }

    }
}
