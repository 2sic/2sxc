using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.WebApiExtensions;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SexyContentApiController
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

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void SetTemplateChooserState([FromUri]bool state)
        {
            ActiveModule.ModuleSettings[SexyContent.SettingsShowTemplateChooser] = state;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableApps()
        {
            var zoneId = SexyContent.GetZoneID(ActiveModule.PortalID);
            return SexyContent.GetApps(zoneId.Value, false, new PortalSettings(ActiveModule.OwnerPortalID));
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void SetAppId(int? appId)
        {
            SexyContent.SetAppIdForModule(ActiveModule, appId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableContentTypes()
        {
            return Sexy.GetAvailableAttributeSetsForVisibleTemplates(PortalSettings.PortalId).Select(p => new { p.AttributeSetID, p.Name } );
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableTemplates()
        {
            IEnumerable<Template> availableTemplates;
            var elements = Sexy.GetContentElements(ActiveModule.ModuleID, Sexy.GetCurrentLanguageName(), null, PortalSettings.PortalId, SexyContent.HasEditPermission(ActiveModule));

            if (elements.Any(e => e.EntityId.HasValue))
                availableTemplates = Sexy.GetCompatibleTemplates(PortalSettings.PortalId, elements.First().GroupId).Where(p => !p.IsHidden);
            else if (elements.Count <= 1)
                availableTemplates = Sexy.GetVisibleTemplates(PortalSettings.PortalId);
            else
                availableTemplates = Sexy.GetVisibleListTemplates(PortalSettings.PortalId);

            return availableTemplates.Select(t => new { t.TemplateID, t.Name, t.AttributeSetID });
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage RenderTemplate([FromUri]int templateId)
        {
            try
            {
                var template = Sexy.TemplateContext.GetTemplate(templateId);
                var engine = EngineFactory.CreateEngine(template);
                var dataSource =
                    (ViewDataSource)
                        Sexy.GetViewDataSource(ActiveModule.ModuleID, SexyContent.HasEditPermission(ActiveModule),
                            DotNetNuke.Common.Globals.IsEditMode(), templateId);
                engine.Init(template, Sexy.App, ActiveModule, dataSource, InstancePurposes.WebView, Sexy);
                engine.CustomizeData();
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(engine.Render(), Encoding.UTF8, "text/plain");
                return response;
            }
            catch (Exception e)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(e);
                throw e;
            }
        }

    }
}