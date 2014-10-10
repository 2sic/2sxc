using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ImportExport;
using System.Linq;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ViewController : DnnApiController
    {

        [HttpGet]
        [AllowAnonymous]
        public string HelloWorld()
        {
            return "Hello";
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void AddItemToList([FromUri]int? sortOrder = null)
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            var appId = SexyContent.GetAppIdFromModule(ActiveModule);

            var sexy = new SexyContent(zoneId.Value, appId.Value, false);
            var elements = sexy.GetContentElements(ActiveModule.ModuleID, sexy.GetCurrentLanguageName(), null, PortalSettings.PortalId, SexyContent.HasEditPermission(ActiveModule)).ToList();
            sexy.AddContentGroupItem(elements.First().GroupId, UserInfo.UserID, elements.First().TemplateId, null, sortOrder.HasValue ? sortOrder.Value + 1 : sortOrder, true, ContentGroupItemType.Content, false);
        }

    }
}