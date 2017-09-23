using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiController
    {
        [HttpGet]
        [HttpPost]
 		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetAvailableEntities([FromUri]int appId, [FromBody] string[] items, [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null) 
            => new Eav.WebApi.EntityPickerController(Log).GetAvailableEntities(appId, items, contentTypeName, dimensionId);
    }
}