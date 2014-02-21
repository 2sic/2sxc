using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Utilities;
using ToSic.Eav;
using ToSic.Eav.ManagementUI;
using DotNetNuke.Entities.Modules;
using System.Web.UI.HtmlControls;
using DotNetNuke.Services.Localization;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public partial class EditContentGroup : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        #region Properties
        /// <summary>
        /// Return the SortOrder from QueryString
        /// </summary>
        public int? SortOrder
        {
            get {
                if (!String.IsNullOrEmpty(Request.QueryString["SortOrder"]))
                    return int.Parse(Request.QueryString["SortOrder"]);
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns the ReturnUrl from QueryString
        /// </summary>
        public string ReturnUrl
        {
            get
            {
                if (String.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                    return DotNetNuke.Common.Globals.NavigateURL(this.TabId);
                else
                    return Request.QueryString["ReturnUrl"];
            }
        }

        public int? LanguageID
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["CultureDimension"]))
                    return int.Parse(Request.QueryString["CultureDimension"]);
                return new int?();
            }
        }

        public int? DefaultLanguageID
        {
            get
            {
                return Sexy.ContentContext.GetLanguageId(PortalSettings.DefaultLanguage);
            }
        }

        /// <summary>
        /// Returns the ContentGroupID from QueryString
        /// </summary>
        public int ContentGroupID
        {
            get
            {
                string ContentGroupIDString = Request.QueryString[SexyContent.ContentGroupIDString];

                if (!String.IsNullOrEmpty(ContentGroupIDString))
                    return int.Parse(ContentGroupIDString);
                else
                    return int.Parse(Settings[SexyContent.ContentGroupIDString].ToString());
            }
        }

        private List<ContentGroupItem> _Items;
        /// <summary>
        /// Returns the current ContentGroupItem
        /// </summary>
        private List<ContentGroupItem> Items
        {
            get
            {
                if (_Items == null)
                    _Items = Sexy.TemplateContext.GetContentGroupItems(ContentGroupID).ToList();
                return _Items;
            }
        }

        private List<ContentGroupItem> CurrentlyEditedItems
        {
            get {
                if (NewMode)
                    return new List<ContentGroupItem>() { new ContentGroupItem()
                    {
                        SortOrder = SortOrder.HasValue ? SortOrder.Value : 0,
                        ContentGroupID = ContentGroupID,
                        Type = ContentGroupItemType.Content.ToString("F"),
                        
                    }};
                else
                return Items.Where(p => p.SortOrder == SortOrder.Value).ToList();
            }
        }

        /// <summary>
        /// Returns true if a new ContentGroupItem should be created at the specified location
        /// </summary>
        private bool NewMode
        {
            get { return Request.QueryString["EditMode"] == "New"; }
        }

        #endregion

        private SexyContent Sexy = new SexyContent();

        protected void Page_Init(object sender, EventArgs e)
        {
            // Register JavaScripts
            ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
            DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration();
        }

        /// <summary>
        /// Handles the control load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            // Add DNN Version to body Class
            Sexy.AddDNNVersionToBodyClass(this);

            // Bind Languages Repeater
            var Languages = Sexy.ContentContext.GetLanguages().Where(l => l.Active).OrderByDescending(l => l.DimensionID == DefaultLanguageID).ThenBy(l => l.ExternalKey);
            if (Languages.Count() == 0)
                pnlDimensionNav.Visible = false;
            rptDimensions.DataSource = Languages;
            rptDimensions.DataBind();

            btnDelete.OnClientClick = "return confirm('" + LocalizeString("btnDelete.Confirm") + "')";
            btnDelete.Text = Items.Count(p => p.ItemType == ContentGroupItemType.Content) > 1 ? LocalizeString("btnDelete.ListText") : LocalizeString("btnDelete.Text");

            // If there is something to edit
            if (CurrentlyEditedItems.Any())
            {
                // Settings link (to change content)
                hlkChangeContent.NavigateUrl = Sexy.GetElementSettingsLink(CurrentlyEditedItems.First().ContentGroupItemID, ModuleId, TabId, Request.RawUrl);

                // Show Change Content or Reference Link only if this is the default language
                var IsDefaultLanguage = LanguageID == DefaultLanguageID;
                hlkChangeContent.Visible = !NewMode && IsDefaultLanguage && (CurrentlyEditedItems.First().ItemType == ContentGroupItemType.Content || CurrentlyEditedItems.First().ItemType == ContentGroupItemType.ListContent);
            }

            if (!Sexy.ContentContext.HasLanguages() || (LanguageID.HasValue && Sexy.ContentContext.GetDimension(LanguageID.Value).Active))
                ProcessView();
            else
            {
                pnlActions.Visible = false;
                pnlLanguageNotActive.Visible = true;
                litLanguageName.Text = LocaleController.Instance.GetLocale(System.Threading.Thread.CurrentThread.CurrentCulture.Name).Text;
                if (UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                    btnActivateLanguage.Visible = true;
            }

            btnDelete.Visible = !NewMode;
        }

        protected void ProcessView()
        {
            List<ContentGroupItemType> EditableItemsTypes = new List<ContentGroupItemType>();

            if (CurrentlyEditedItems.Any() && CurrentlyEditedItems.Any(c => c.ItemType == ContentGroupItemType.Content))
            {
                EditableItemsTypes.Add(ContentGroupItemType.Content);
                EditableItemsTypes.Add(ContentGroupItemType.Presentation);
            }
            if (SortOrder == -1 || CurrentlyEditedItems.Any(c => c.ItemType == ContentGroupItemType.ListContent))
            {
                EditableItemsTypes.Add(ContentGroupItemType.ListContent);
                EditableItemsTypes.Add(ContentGroupItemType.ListPresentation);
            }

            if (Items.Any() && Items.First().TemplateID.HasValue)
            {
                foreach (var TemplateDefault in Sexy.GetTemplateDefaults(Items.First().TemplateID.Value).Where(c => EditableItemsTypes.Contains(c.ItemType)))
                {
                    if (TemplateDefault.ContentTypeID.HasValue && TemplateDefault.ContentTypeID.Value > 0)
                    {
                        ContentGroupItem ContentGroupItem = null;
                        if (CurrentlyEditedItems.Any() && CurrentlyEditedItems.First().ContentGroupItemID != 0)
                            ContentGroupItem = CurrentlyEditedItems.FirstOrDefault(p => p.ItemType == TemplateDefault.ItemType);

                        EditContentGroupItem EditControl = (EditContentGroupItem)LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "EditContentGroupItem.ascx"));
                        EditControl.ContentGroupItemID = ContentGroupItem != null && ContentGroupItem.ContentGroupID != 0 ? ContentGroupItem.ContentGroupItemID : new int?();
                        EditControl.ContentGroupID = ContentGroupID;
                        EditControl.ItemType = TemplateDefault.ItemType;
                        EditControl.TemplateID = Items.First().TemplateID.Value;
                        EditControl.SortOrder = CurrentlyEditedItems.Any() ? SortOrder : new int?();
                        EditControl.ModuleID = ModuleId;
                        EditControl.TabID = TabId;
                        EditControl.AttributeSetID = TemplateDefault.ContentTypeID.Value;
                        phNewOrEditControls.Controls.Add(EditControl);
                    }
                }
            }

        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach(EditContentGroupItem EditControl in phNewOrEditControls.Controls)
                EditControl.Save();

            RedirectBack();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (EditContentGroupItem EditControl in phNewOrEditControls.Controls)
                EditControl.Cancel();

            RedirectBack();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            new SexyContent(false).TemplateContext.DeleteContentGroupItems(ContentGroupID, SortOrder.Value, UserId);
            RedirectBack();
        }

        protected void RedirectBack()
        {
            if (!String.IsNullOrEmpty(ReturnUrl))
                Response.Redirect(ReturnUrl, true);
            else
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId), true);
        }

        protected void rptDimensions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "ChangeLanguage")
            {
                if (Boolean.Parse(hfMustSave.Value))
                {
                    // Save when changing language
                    foreach (EditContentGroupItem EditControl in phNewOrEditControls.Controls)
                        EditControl.Save();
                }

                var Url = GetCultureUrl(int.Parse(e.CommandArgument.ToString()));
                Response.Redirect(Url);
            }
        }

        protected string GetCultureUrl(int cultureDimensionId)
        {
            // Create URL for other language
            var Url = Request.RawUrl;
            Url = Regex.Replace(Url, "&CultureDimension=[0-9]+?", "");
            Url = Url + "&CultureDimension=" + cultureDimensionId;

            return Url;
        }

        protected void btnActivateLanguage_Click(object sender, EventArgs e)
        {
            Sexy.SetCultureState(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true, PortalId);
            Response.Redirect(Sexy.GetElementEditLink(ContentGroupID, SortOrder.Value, ModuleId, TabId, ""));
        }
    }
}