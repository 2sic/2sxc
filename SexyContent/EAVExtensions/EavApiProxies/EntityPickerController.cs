using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : DnnApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetAvailableEntities(int zoneId, int appId, int? attributeSetId = null, int? dimensionId = null)
        {
            return new ToSic.Eav.ManagementUI.API.EntityPickerController().GetAvailableEntities(zoneId, appId, attributeSetId, dimensionId);
        }

    }
}