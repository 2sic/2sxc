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
using ToSic.Eav.DataSources;
using ToSic.Eav.ManagementUI;
using DotNetNuke.Entities.Modules;
using System.Web.UI.HtmlControls;
using DotNetNuke.Services.Localization;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    /// <summary>
    /// Edit Entities - either by specifying SortOrder of the current ContentGroup, or by specifying the Entity ID directly.
    /// If AttributeSetId is specified in the url, this control will allow editing the entity (or create new)
    /// If ContentGroupId is specified in the url, this control will allow editing the ContentGroupItem (or create new)
    /// </summary>
    public partial class EditContentGroup : SexyControlEditBase
    {
        #region Properties
        /// <summary>
        /// Return the SortOrder from QueryString
        /// </summary>
        public int? SortOrder
        {
            get {
                if (!String.IsNullOrEmpty(Request.QueryString["SortOrder"]))
                {
                    int sortOrder;
                    if (int.TryParse(Request.QueryString["SortOrder"], out sortOrder))
                        return sortOrder;
                }
                return new int?();
            }
        }

        /// <summary>
        /// Return the EntityID from QueryString
        /// </summary>
        public int? EntityId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["EntityId"]))
                    return int.Parse(Request.QueryString["EntityId"]);
                return new int?();
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
                {
                    int languageId;
                    if (int.TryParse(Request.QueryString["CultureDimension"], out languageId))
                        return languageId;
                }
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
        public int? ContentGroupID
        {
            get
            {
                string ContentGroupIDString = Request.QueryString[SexyContent.ContentGroupIDString];

                if (!String.IsNullOrEmpty(ContentGroupIDString))
                    return int.Parse(ContentGroupIDString);

                return new int?();
            }
        }


        private int? _attributeSetId;
        /// <summary>
        /// Returns the AttributeSetId from QueryString
        /// </summary>
        public int? AttributeSetId
        {
            get
            {
                if (_attributeSetId == null)
                {
                    string attributeSetName = Request.QueryString["AttributeSetName"];

                    if (!String.IsNullOrEmpty(attributeSetName))
                        _attributeSetId = Sexy.ContentContext.GetAllAttributeSets().FirstOrDefault(p => p.Name == attributeSetName || p.StaticName == attributeSetName).AttributeSetID;
                    else if (!String.IsNullOrWhiteSpace(Request.QueryString["AttributeSetId"]))
                        _attributeSetId = int.Parse(Request.QueryString["AttributeSetId"]);
                }
                return _attributeSetId;
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
                if (_Items == null && ContentGroupID.HasValue)
                    _Items = Sexy.TemplateContext.GetContentGroupItems(ContentGroupID.Value).ToList();
                if(_Items == null && (AttributeSetId.HasValue || EntityId.HasValue))
                    _Items = new List<ContentGroupItem>();
                return _Items;
            }
        }

        private List<ContentGroupItem> CurrentlyEditedItems
        {
            get {
                if (NewMode && ContentGroupID.HasValue)
                    return new List<ContentGroupItem>() { new ContentGroupItem()
                    {
                        SortOrder = SortOrder.HasValue ? SortOrder.Value : 0,
                        ContentGroupID = ContentGroupID.Value,
                        Type = ContentGroupItemType.Content.ToString("F"),
                        
                    }};
                return !SortOrder.HasValue ? new List<ContentGroupItem>() : Items.Where(p => p.SortOrder == SortOrder.Value).ToList();
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

        protected void Page_Init(object sender, EventArgs e)
        {
            // Register JavaScripts
            ClientAPI.RegisterClientReference(this.Page, ClientAPI.ClientNamespaceReferences.dnn);
            DotNetNuke.Framework.jQuery.RequestDnnPluginsRegistration();

            base.Page_Init(sender, e);
        }

        /// <summary>
        /// Handles the control load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            // Add DNN Version to body Class
            SexyContent.AddDNNVersionToBodyClass(this);

            // Bind Languages Repeater
            var Languages = Sexy.ContentContext.GetLanguages().Where(l => l.Active).OrderByDescending(l => l.DimensionID == DefaultLanguageID).ThenBy(l => l.ExternalKey);
            if (Languages.Count() == 0)
                pnlDimensionNav.Visible = false;
            rptDimensions.DataSource = Languages;
            rptDimensions.DataBind();

            btnDelete.OnClientClick = "return confirm('" + LocalizeString("btnDelete.Confirm") + "')";
            btnDelete.Text = Items.Count(p => p.ItemType == ContentGroupItemType.Content) > 1 ? LocalizeString("btnDelete.ListText") : LocalizeString("btnDelete.Text");
            btnDelete.Visible = !NewMode && ContentGroupID.HasValue;

            // If there is something to edit
            if (CurrentlyEditedItems.Any())
            {
                // Settings link (to change content)
                hlkChangeContent.NavigateUrl = Sexy.GetElementSettingsLink(CurrentlyEditedItems.First().ContentGroupItemID, ModuleId, TabId, Request.RawUrl);

                // Show Change Content or Reference Link only if this is the default language
                var IsDefaultLanguage = LanguageID == DefaultLanguageID;
                hlkChangeContent.Visible = !NewMode && IsDefaultLanguage && (CurrentlyEditedItems.First().ItemType == ContentGroupItemType.Content || CurrentlyEditedItems.First().ItemType == ContentGroupItemType.ListContent);
            }

            // Show message if language is not active
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

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            chkPublished.Checked = ((IEditContentControl)phNewOrEditControls.Controls[0]).IsPublished;
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

                        var editControl = (EditContentGroupItem)LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "EditContentGroupItem.ascx"));
                        editControl.ContentGroupItemID = ContentGroupItem != null && ContentGroupItem.ContentGroupID != 0 ? ContentGroupItem.ContentGroupItemID : new int?();
                        editControl.ContentGroupID = ContentGroupID.Value;
                        editControl.AppId = AppId.Value;
                        editControl.ZoneId = ZoneId.Value;
                        editControl.ItemType = TemplateDefault.ItemType;
                        editControl.TemplateID = Items.First().TemplateID.Value;
                        editControl.SortOrder = CurrentlyEditedItems.Any() || SortOrder == -1 ? SortOrder : new int?();
                        editControl.ModuleID = ModuleId;
                        editControl.TabID = TabId;
                        editControl.AttributeSetID = TemplateDefault.ContentTypeID.Value;
                        phNewOrEditControls.Controls.Add(editControl);
                    }
                }
            }

            // Directly edit entity Id
            if ((!SortOrder.HasValue && EntityId.HasValue) || (!SortOrder.HasValue && AttributeSetId.HasValue && NewMode))
            {
                var editControl = (EditEntity)LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "EditEntity.ascx"));
                editControl.AppId = AppId.Value;
                editControl.ZoneId = ZoneId.Value;
                editControl.ModuleID = ModuleId;
                editControl.TabID = TabId;
                editControl.AttributeSetID = AttributeSetId.HasValue ? AttributeSetId.Value : Sexy.ContentContext.GetEntity(EntityId.Value).AttributeSetID;
                editControl.EntityId = EntityId;
                phNewOrEditControls.Controls.Add(editControl);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (IEditContentControl EditControl in phNewOrEditControls.Controls)
            {
                EditControl.IsPublished = chkPublished.Checked;
                EditControl.Save();
            }

            RedirectBack();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (IEditContentControl EditControl in phNewOrEditControls.Controls)
            {
                EditControl.Cancel();
            }

            RedirectBack();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SexyUncached.TemplateContext.DeleteContentGroupItems(ContentGroupID.Value, SortOrder.Value, UserId);
            RedirectBack();
        }

        protected void RedirectBack()
        {
            if (Request.QueryString["PreventRedirect"] == "true")
                return;

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
                    foreach (IEditContentControl EditControl in phNewOrEditControls.Controls)
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
            Url = Regex.Replace(Url, "&CultureDimension=[0-9]+", "", RegexOptions.IgnoreCase);
            Url = Url + "&CultureDimension=" + cultureDimensionId;

            return Url;
        }

        protected void btnActivateLanguage_Click(object sender, EventArgs e)
        {
            Sexy.SetCultureState(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true, PortalId);
            Response.Redirect(Sexy.GetElementEditLink(ContentGroupID.Value, SortOrder.Value, ModuleId, TabId, ""));
        }
    }
}