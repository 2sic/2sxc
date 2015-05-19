using System;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Web.UI.WebControls;
using Telerik.Web.UI;

namespace ToSic.SexyContent
{
    public partial class ManageTemplates : SexyControlAdminBase
    {
        /// <summary>
        /// Set the localized ConfirmText when deleting a template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            ((DnnGridButtonColumn)grdTemplates.Columns.FindByUniqueName("DeleteColumn")).ConfirmText = LocalizeString("DeleteColumn.ConfirmText");
			grdTemplates.Columns.FindByUniqueName("PermissionsColumn").Visible = !IsContentApp;
	        base.Page_Init(sender, e);
        }

        /// <summary>
        /// Bind the grid and set NavigateUrls's (for DotNetNuke Modal Window)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindGrdTemplates();

            hlkNewTemplate.NavigateUrl = EditUrl(PortalSettings.ActiveTab.TabID, SexyContent.ControlKeys.EditTemplate, true, "mid=" + ModuleId + "&" + SexyContent.AppIDString + "=" + AppId);
            hlkCancel.NavigateUrl = Globals.NavigateURL(TabId, "", null);

            if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId))
                pnlSexyContentDesignersInfo.Visible = true;
        }

        /// <summary>
        /// GridView DeleteCommand, deletes the template that caused the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTemplates_DeleteCommand(object sender, GridCommandEventArgs e)
        {
			var templateId = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][SexyContent.TemplateID]);
			Sexy.Templates.DeleteTemplate(templateId);
			BindGrdTemplates();
        }

        /// <summary>
        /// Re-Bind templates-grid after sort-command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTemplates_SortCommand(object sender, EventArgs e)
        {
            BindGrdTemplates();
        }

        /// <summary>
        /// Redirect to the edit template window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTemplates_EditCommand(object sender, GridCommandEventArgs e)
        {
            var templateId = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][SexyContent.TemplateID]);
            var editUrl = ModuleContext.NavigateUrl(TabId, SexyContent.ControlKeys.EditTemplate, true, "mid" + "=" + ModuleId + "&" + SexyContent.TemplateID + "=" + templateId + "&" + SexyContent.AppIDString + "=" + AppId);
            Response.Redirect(editUrl);
        }

		//public string PermissionsLink(string templateGuid)
		//{
		//	var editUrl = ModuleContext.EditUrl(SexyContent.ControlKeys.Permissions, 
		//		"mid" + "=" + ModuleId + "&Target=" + templateGuid + "&" + SexyContent.AppIDString + "=" + AppId);
		//	return editUrl;
		//}

        
        /// <summary>
        /// DataBind the template grid view
        /// </summary>
        private void BindGrdTemplates()
        {
            var attributeSetList = Sexy.GetAvailableContentTypes(SexyContent.AttributeSetScope).ToList();
            var templateList = Sexy.Templates.GetAllTemplates();
            var templates = from c in  templateList
                            join a in attributeSetList on c.ContentTypeStaticName equals a.StaticName into JoinedList
                            from a in JoinedList.DefaultIfEmpty()
                            select new
                            {
                                TemplateID = c.TemplateId,
                                TemplateName = c.Name, c.ContentTypeStaticName,
                                AttributeSetName = a != null ? a.Name : "No Content Type",
                                TemplatePath = c.Path,
                                DemoEntityID = c.ContentDemoEntity != null ? c.ContentDemoEntity.EntityId : new int?(), 
                                c.IsHidden,
                                c.ViewNameInUrl,
                                c.Guid
                            };

            grdTemplates.DataSource = templates;
            grdTemplates.DataBind();
        }
    }
}