using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

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
            //Sexy = new SexyContent(this.GetZoneId(), Request.QueryString.AllKeys.Contains("AppID") ? int.Parse(Request.QueryString["AppID"]) : new int?(), true);

            ((DnnGridButtonColumn)grdTemplates.Columns.FindByUniqueName("DeleteColumn")).ConfirmText = LocalizeString("DeleteColumn.ConfirmText");
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

            hlkNewTemplate.NavigateUrl = EditUrl(PortalSettings.ActiveTab.TabID, SexyContent.ControlKeys.EditTemplate, true, "mid=" + this.ModuleId + "&" + SexyContent.AppIDString + "=" + AppId);
            hlkCancel.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null);

            if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId))
                pnlSexyContentDesignersInfo.Visible = true;
        }

        /// <summary>
        /// GridView DeleteCommand, deletes the template that caused the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdTemplates_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int TemplateID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][SexyContent.TemplateID]);
            SexyUncached.TemplateContext.DeleteTemplate(TemplateID, UserId);
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
        protected void grdTemplates_EditCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int TemplateID = Convert.ToInt32(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex][SexyContent.TemplateID]);
            string EditUrl = ModuleContext.NavigateUrl(TabId, SexyContent.ControlKeys.EditTemplate.ToString(), true, "mid" + "=" + ModuleId.ToString() + "&" + SexyContent.TemplateID + "=" + TemplateID.ToString() + "&" + SexyContent.AppIDString + "=" + AppId.ToString());
            Response.Redirect(EditUrl);
        }

        /// <summary>
        /// DataBind the template grid view
        /// </summary>
        private void BindGrdTemplates()
        {
            var AttributeSetList = Sexy.GetAvailableAttributeSets(SexyContent.AttributeSetScope).ToList();
            var TemplateList = Sexy.GetTemplates(PortalId).ToList();
            var Templates = from c in  TemplateList
                            join a in AttributeSetList on c.AttributeSetID equals a.AttributeSetID into JoinedList
                            from a in JoinedList.DefaultIfEmpty()
                            select new
                            {
                                TemplateID = c.TemplateID,
                                TemplateName = c.Name,
                                AttributeSetID = c.AttributeSetID,
                                AttributeSetName = a != null ? a.Name : "No Content Type",
                                TemplatePath = c.Path,
                                DemoEntityID = c.DemoEntityID,
                                IsHidden = c.IsHidden
                            };

            grdTemplates.DataSource = Templates;
            grdTemplates.DataBind();
        }
    }
}