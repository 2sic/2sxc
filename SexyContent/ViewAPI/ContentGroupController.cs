using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : DnnApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void AddItem([FromUri] int? sortOrder = null)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);

            var sexy = new SexyContent(zoneId.Value, appId.Value, false);
            var elements = sexy.GetContentElements(ActiveModule.ModuleID, sexy.GetCurrentLanguageName(), null, PortalSettings.PortalId, SexyContent.HasEditPermission(ActiveModule)).ToList();
            sexy.AddContentGroupItem(elements.First().GroupId, UserInfo.UserID, elements.First().TemplateId, null, sortOrder.HasValue ? sortOrder.Value + 1 : sortOrder, true, ContentGroupItemType.Content, false);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void ChangeTemplate([FromUri] int? templateId)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);
            var sexy = new SexyContent(zoneId.Value, appId.Value, false);

            sexy.UpdateTemplateForGroup(sexy.GetContentGroupIdFromModule(ActiveModule.ModuleID), templateId, UserInfo.UserID);
        }

    }
}