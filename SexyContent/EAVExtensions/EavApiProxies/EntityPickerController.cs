using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetAvailableEntities(string entityType = null, int? dimensionId = null)
        {
            return new Eav.ManagementUI.API.EntityPickerController().GetAvailableEntities(App.ZoneId, App.AppId, entityType, dimensionId);
        }

    }
}