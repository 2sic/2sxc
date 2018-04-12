using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiController
    {
        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
 		//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<dynamic> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
        {
            // do security check
            GetAppRequiringPermissionsOrThrow(appId, GrantSets.ReadSomething, contentTypeName);

            // maybe in the future, ATM not relevant
            //var withDrafts = set.Item2.UserMay(GrantSets.ReadDraft);

            return new Eav.WebApi.EntityPickerController(Log)
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);
        }
    }
}