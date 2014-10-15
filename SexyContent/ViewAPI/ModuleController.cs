using System.Collections.Generic;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.Eav;
using ToSic.SexyContent.WebApiExtensions;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SexyContentApiController
    {

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
        public IEnumerable<object> GetSelectableContentTypes()
        {
            return Sexy.GetAvailableAttributeSetsForVisibleTemplates(PortalSettings.PortalId).Select(p => new { p.AttributeSetID, p.Name } );
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableTemplates([FromUri]int? attributeSetId = new int?())
        {
            IEnumerable<Template> availableTemplates;
            var elements = Sexy.GetContentElements(ActiveModule.ModuleID, Sexy.GetCurrentLanguageName(), null, PortalSettings.PortalId, SexyContent.HasEditPermission(ActiveModule));

            if (elements.Any(e => e.EntityId.HasValue))
                availableTemplates = Sexy.GetCompatibleTemplates(PortalSettings.PortalId, elements.First().GroupId).Where(p => !p.IsHidden);
            else if (elements.Count <= 1)
                availableTemplates = Sexy.GetVisibleTemplates(PortalSettings.PortalId);
            else
                availableTemplates = Sexy.GetVisibleListTemplates(PortalSettings.PortalId);

            // Make only templates from chosen content type shown if content type is set
            if (attributeSetId.HasValue)
                availableTemplates = availableTemplates.Where(p => p.AttributeSetID == attributeSetId.Value);


            //// If the current data is a list of entities, don't allow changing back to no template
            //if (Elements.Count <= 1 && (!Elements.Any(e => e.EntityId.HasValue)))
            //    ddlTemplate.Items.Insert(0, new ListItem(LocalizeString("ddlTemplateDefaultItem.Text"), "0"));

            //// If there are elements and the selected template exists in the list, select that
            //if (Elements.Any() && ddlTemplate.Items.FindByValue(Elements.First().TemplateId.ToString()) != null)
            //    ddlTemplate.SelectedValue = Elements.First().TemplateId.ToString();
            //else
            //{
            //    if (ddlContentType.SelectedValue != "0")
            //    {
            //        if (ddlTemplate.Items.Count > 1)
            //            ddlTemplate.SelectedIndex = 1;

            //        ChangeTemplate();
            //    }
            //}

            return availableTemplates.Select(t => new { t.TemplateID, t.Name });
        }

    }
}