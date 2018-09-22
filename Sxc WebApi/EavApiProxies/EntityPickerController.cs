using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiControllerBase
    {
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
 		//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<dynamic> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
        {
            // do security check
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, contentTypeName, Log);
            if(!permCheck.EnsureAll(GrantSets.ReadSomething, out var exp))
                throw exp;

            // maybe in the future, ATM not relevant
            //var withDrafts = set.Item2.UserMay(GrantSets.ReadDraft);

            return new Eav.WebApi.EntityPickerController(Log)
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);
        }
    }
}