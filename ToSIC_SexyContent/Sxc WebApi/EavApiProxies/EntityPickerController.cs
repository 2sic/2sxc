using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class EntityPickerController : SxcApiControllerBase, IEntityPickerController
    {
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
 		//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<dynamic> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
        {
            // do security check
            var permCheck = string.IsNullOrEmpty(contentTypeName) 
                ? new MultiPermissionsApp(CmsBlock, appId, Log)
                : new MultiPermissionsTypes(CmsBlock, appId, contentTypeName, Log);
            if(!permCheck.EnsureAll(GrantSets.ReadSomething, out var exp))
                throw exp;

            // maybe in the future, ATM not relevant
            //var withDrafts = set.Item2.UserMay(GrantSets.ReadDraft);

            return new Eav.WebApi.EntityPickerController(Log)
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);
        }
    }
}