using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class EntityPickerController : SxcApiControllerBase, IEntityPickerController
    {
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
 		//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<EntityForPickerDto> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
        {
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName) 
                ? new MultiPermissionsApp(BlockBuilder, appId, Log)
                : new MultiPermissionsTypes(BlockBuilder, appId, contentTypeName, Log);
            if(!permCheck.EnsureAll(GrantSets.ReadSomething, out var exp))
                throw exp;

            // maybe in the future, ATM not relevant
            var withDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);

            return new Eav.WebApi.EntityPickerController(Log)
                .GetAvailableEntities(appId, items, contentTypeName, withDrafts, dimensionId);
        }
    }
}