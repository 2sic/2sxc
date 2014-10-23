using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using DotNetNuke.Entities.Modules;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Change Layout or Content
    /// </summary>
    public partial class SettingsWrapper : SexyControlEditBase
    {
        #region Private Properties
        private IEnumerable<ContentGroupItem> _ContentGroupItems;
        private IEnumerable<ContentGroupItem> ContentGroupItems
        {
            get
            {
                if (_ContentGroupItems == null)
                {
                    if (DirectEditContentGroupItem)
                        _ContentGroupItems = SexyUncached.TemplateContext.GetContentGroupItems(ContentGroupItem.ContentGroupID);
                    else
                        _ContentGroupItems = SexyUncached.TemplateContext.GetContentGroupItems(SexyUncached.GetContentGroupIdFromModule(ModuleId));
                }
                return _ContentGroupItems;
            }
        }

        private ContentGroupItem _ContentGroupItem;
        private ContentGroupItem ContentGroupItem
        {
            get
            {
                if (_ContentGroupItem == null)
                {
                    if (DirectEditContentGroupItem)
                        _ContentGroupItem = SexyUncached.TemplateContext.GetContentGroupItem(int.Parse(Request.QueryString[SexyContent.ContentGroupItemIDString]));
                    else if (ContentGroupItems.Count() != 0)
                        _ContentGroupItem = ContentGroupItems.First();
                }
                return _ContentGroupItem;
            }
        }

        private bool DirectEditContentGroupItem
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString[SexyContent.ContentGroupItemIDString]);
            }
        }
        #endregion

        /// <summary>
        /// Page init for the settingswrapper control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            // Nothing to do if PostBack
            if (Page.IsPostBack)
                return;

            // Break if there is no ContentGroupItem
            if (ContentGroupItem == null)
                return;

            int? TemplateID = ContentGroupItem.TemplateID;
            int? EntityID = ContentGroupItem.EntityID;
            int? AttributeSetID = null;

            // If TemplateID is set, fill template-dropdown with templates of the same 
            // AttributeSetId / Content Type. Else show all visible templates.
            if (TemplateID.HasValue)
            {
                AttributeSetID = SexyUncached.GetTemplateDefault(TemplateID.Value, ContentGroupItem.ItemType).ContentTypeID;

                lblContentTypeText.Text = Sexy.ContentContext.GetAttributeSet(AttributeSetID.Value).Name;
                lblContentTypeText.Visible = true;
                lblContentTypeDefaultText.Visible = false;

                // ToDo: This dialog should not be used to change the template anymore, clean it up!
                //IEnumerable<Template> CompatibleTemplates = SexyUncached.GetCompatibleTemplates(PortalId, ContentGroupItem.ContentGroupID).Where(p => !p.IsHidden);
                //ddlTemplate.DataSource = CompatibleTemplates;

                //if (CompatibleTemplates.Any(p => p.TemplateID == TemplateID))
                //    ddlTemplate.SelectedValue = TemplateID.ToString();
            }
            else
                ddlTemplate.DataSource = SexyUncached.GetVisibleTemplates(PortalId);

            ddlTemplate.DataBind();

            if (DirectEditContentGroupItem)
            {
                pnlTemplate.Visible = false;
                ddlEntities.Items.RemoveAt(0);
            }

            if (ContentGroupItems.Count(c => c.ItemType == ContentGroupItemType.Content) > 1 && !DirectEditContentGroupItem)
                pnlEntities.Visible = false;
            else
            {
                // If Content Type is set, show available Entities
                if (AttributeSetID.HasValue)
                {
                    ddlEntities.DataSource = from System.Data.DataRow c in Sexy.ContentContext.GetItemsTable(AttributeSetID.Value).AsEnumerable()
                                             select new
                                             {
                                                 EntityTitle = c["EntityTitle"] + " (" + c["EntityID"] + ")",
                                                 EntityID = c["EntityID"]
                                             };
                    ddlEntities.DataBind();
                }

                // If EntityID is set, select the chosen Entity in the dropdown
                if (EntityID.HasValue)
                    ddlEntities.SelectedValue = EntityID.Value.ToString();
            }
        }

        /// <summary>
        /// Call "UpdateSettings" on the settings control and close the modal window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Save Template
            if (ddlTemplate.SelectedValue != "-1")
                SexyUncached.UpdateTemplateForGroup(ContentGroupItem.ContentGroupID, int.Parse(ddlTemplate.SelectedValue), UserId);

            // Save Entity (if not list)
            if (ContentGroupItems.Count(c => c.ItemType == ContentGroupItemType.Content) < 2 || DirectEditContentGroupItem)
            {
                if (ddlEntities.SelectedValue != "-1")
                    ContentGroupItem.EntityID = int.Parse(ddlEntities.SelectedValue);
                else
                    ContentGroupItem.EntityID = null;
                ContentGroupItem.SysModified = DateTime.Now;
                ContentGroupItem.SysModifiedBy = UserId;
            }

            SexyUncached.TemplateContext.SaveChanges();

            RedirectReturn();
        }

        /// <summary>
        /// Close the modal window without saving the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectReturn();
        }

        private void RedirectReturn()
        {
            if (String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                Response.Redirect(ModuleContext.NavigateUrl(this.TabId, "", true));
            else
                Response.Redirect(Request.QueryString["ReturnUrl"]);
        }
    }
}