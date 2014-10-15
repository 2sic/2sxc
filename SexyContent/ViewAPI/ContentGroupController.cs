using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.SexyContent.WebApiExtensions;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : SexyContentApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void AddItem([FromUri] int? sortOrder = null)
        {
            var elements = Sexy.GetContentElements(ActiveModule.ModuleID, Sexy.GetCurrentLanguageName(), null, PortalSettings.PortalId, SexyContent.HasEditPermission(ActiveModule)).ToList();
            SexyUncached.AddContentGroupItem(elements.First().GroupId, UserInfo.UserID, elements.First().TemplateId, null, sortOrder.HasValue ? sortOrder.Value + 1 : sortOrder, true, ContentGroupItemType.Content, false);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void SaveTemplateId([FromUri] int? templateId)
        {
            SexyUncached.UpdateTemplateForGroup(Sexy.GetContentGroupIdFromModule(ActiveModule.ModuleID), templateId, UserInfo.UserID);
        }

    }
}