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
                    _ContentGroupItems = SexyUncached.TemplateContext.GetContentGroupItems(ContentGroupItem.ContentGroupID);
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
                    _ContentGroupItem = SexyUncached.TemplateContext.GetContentGroupItem(int.Parse(Request.QueryString[SexyContent.ContentGroupItemIDString]));
                }
                return _ContentGroupItem;
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
            if (ContentGroupItem == null || !ContentGroupItem.TemplateID.HasValue)
                return;

            int? entityID = ContentGroupItem.EntityID;
            int? attributeSetID = SexyUncached.GetTemplateDefault(ContentGroupItem.TemplateID.Value, ContentGroupItem.ItemType).ContentTypeID;

            if (attributeSetID.HasValue)
            {
                ddlEntities.DataSource =
                    Sexy.ContentContext.GetItemsTable(attributeSetID.Value).AsEnumerable().Select(i => new
                    {
                        EntityTitle = i["EntityTitle"] + " (" + i["EntityID"] + ")",
                        EntityID = i["EntityID"]
                    });
                ddlEntities.DataBind();
            }

            if (entityID.HasValue)
                ddlEntities.SelectedValue = entityID.Value.ToString();

        }

        /// <summary>
        /// Call "UpdateSettings" on the settings control and close the modal window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            if (ddlEntities.SelectedValue != "-1")
                ContentGroupItem.EntityID = int.Parse(ddlEntities.SelectedValue);
            else
                ContentGroupItem.EntityID = null;
            ContentGroupItem.SysModified = DateTime.Now;
            ContentGroupItem.SysModifiedBy = UserId;

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