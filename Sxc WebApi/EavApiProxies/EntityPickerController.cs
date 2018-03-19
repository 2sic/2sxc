using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiController
    {
        [HttpGet]
        [HttpPost]
 		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public IEnumerable<dynamic> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
        {
            // do security check
            var set = GetAppRequiringPermissionsOrThrow(appId, GrantSets.ReadAnything);

            var withDrafts = set.Item2.UserMay(GrantSets.ReadDrafts);

            return new Eav.WebApi.EntityPickerController(Log)
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);
        }
    }
}