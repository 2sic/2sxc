using System.Collections.Generic;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent.EAVExtensions.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    public class EntityPickerController : SxcApiController
    {
 		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<dynamic> GetAvailableEntities(int appId, string contentTypeName = null,
            int? dimensionId = null)
        {
			return new Eav.WebApi.EntityPickerController().GetAvailableEntities(App.AppId, contentTypeName, dimensionId);
        }
    }
}