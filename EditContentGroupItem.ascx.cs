using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.Eav.ManagementUI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public partial class EditContentGroupItem : PortalModuleBase, IEditContentControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ContentGroupItemID
        /// </summary>
        public int? ContentGroupItemID { get; set; }

        /// <summary>
        /// Gets or sets the ContentGroupID
        /// </summary>
        public int ContentGroupID { get; set; }

        public bool IsPublished
        {
            get { return EditItemControl.IsPublished; }
            set { EditItemControl.IsPublished = value; }
        }

        /// <summary>
        /// Gets or sets the ItemType
        /// </summary>
        public ContentGroupItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the TemplateID
        /// </summary>
        public int TemplateID { get; set; }

        /// <summary>
        /// Gets or sets the SortOrder
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets ModuleID
        /// </summary>
        public int ModuleID { get; set; }

        /// <summary>
        /// Gets or sets TabID
        /// </summary>
        public int TabID { get; set; }

        public int AttributeSetID { get; set; }

        public int ZoneId { get; set;}
        public int AppId { get; set; }

        private SexyContent _sexy;
        public SexyContent Sexy {
            get {
                if (_sexy == null)
                {
                    if (ZoneId == 0 || AppId == 0)
                        throw new ArgumentNullException("ZoneId and AppId must be set.");
                    _sexy = new SexyContent(ZoneId, AppId);
                }
                return _sexy;
            }
        }
        private SexyContent _sexyUncached;
        public SexyContent SexyUncached {
            get {
                if (_sexyUncached == null)
                    _sexyUncached = new SexyContent(ZoneId, AppId, false);
                return _sexyUncached;
            }
        }

        private ContentGroupItem _Item;
        /// <summary>
        /// Returns the current ContentGroupItem
        /// </summary>
        private ContentGroupItem Item
        {
            get
            {
                if (_Item == null && ContentGroupItemID.HasValue)
                    _Item = Sexy.TemplateContext.GetContentGroupItem(ContentGroupItemID.Value);
                return _Item;
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

        #endregion


        //private SexyContent Sexy = new SexyContent(false);
        private ItemForm EditItemControl;

        public EventHandler OnSaved;
        
        /// <summary>
        /// Handles the control load. Will place NewItem or EditItem control into the placeholder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Make sure correct LocalResourceFile gets loaded
            LocalResourceFile = LocalResourceFile + "EditContentGroupItem.ascx";

            ProcessView();

            hSectionHead.ID = "SexyContent-EditSection-" + ItemType.ToString("F");

            if (IsPostBack)
                return;

            if (Item != null)
            {
                // Show Change Content or Reference Link only if this is the default language
                var IsDefaultLanguage = LanguageID == DefaultLanguageID;
                btnReference.Visible = IsDefaultLanguage && (Item.ItemType != ContentGroupItemType.Content && ItemType != ContentGroupItemType.ListContent);
                lblNewOrEditItemHeading.Attributes.Add("title", Item.EntityID.HasValue ? Item.EntityID.Value.ToString() : "");
            }

            lblNewOrEditItemHeading.Text = ItemType.ToString("F");
        }

        protected void ProcessView()
        {
            EditItemControl = (ItemForm)LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "SexyContent/EAV/Controls/ItemForm.ascx"));
            EditItemControl.DefaultCultureDimension = DefaultLanguageID != 0 ? DefaultLanguageID : new int?();
            EditItemControl.IsDialog = false;
            EditItemControl.HideNavigationButtons = true;
            EditItemControl.PreventRedirect = true;
            EditItemControl.AttributeSetId = AttributeSetID;
            EditItemControl.AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault;
            EditItemControl.ZoneId = ZoneId;
            EditItemControl.AppId = AppId;
	        EditItemControl.AddClientScriptAndCss = false;
            EditItemControl.ItemHistoryUrl = "";

            // If ContentGroupItem has Entity, edit that; else create new Entity
            if (Item != null && Item.EntityID.HasValue)
            {
                EditItemControl.Updated += EditItem_OnEdited;
                EditItemControl.EntityId = Item.EntityID.Value;
                EditItemControl.InitForm(FormViewMode.Edit);

                hlkHistory.Visible = true;
                hlkHistory.NavigateUrl = EditUrl("", "", SexyContent.ControlKeys.EavManagement, new string[] { "AppID", AppId.ToString(), "ManagementMode", "ItemHistory", "EntityId", Item.EntityID.Value.ToString(), "mid", ModuleID.ToString() });
            }
            // Create a new Entity
            else
            {
                EditItemControl.Inserted += NewItem_OnInserted;
                EditItemControl.Visible = false;

                if (ItemType == ContentGroupItemType.Content || ItemType == ContentGroupItemType.ListContent)
                    EditItemControl.Visible = true;
                else
                    pnlReferenced.Visible = true;

                EditItemControl.InitForm(FormViewMode.Insert);
            }

            phNewOrEditItem.Controls.Add(EditItemControl);
            
        }

        protected void EditItem_OnEdited(ToSic.Eav.Entity Entity)
        {
            UpdateModuleTitleIfNecessary(Entity, Item);
        }

        protected void NewItem_OnInserted(ToSic.Eav.Entity Entity)
        {
            ContentGroupItem NewItem;

            if (Item != null)
                NewItem = SexyUncached.TemplateContext.GetContentGroupItem(Item.ContentGroupItemID);
            else
                NewItem = SexyUncached.AddContentGroupItem(ContentGroupID, UserId, TemplateID, Entity.EntityID, SortOrder, true, ItemType, ItemType != ContentGroupItemType.Content);

            NewItem.EntityID = Entity.EntityID;
            NewItem.SysModified = DateTime.Now;
            NewItem.SysModifiedBy = UserId;
            SexyUncached.TemplateContext.SaveChanges();

            UpdateModuleTitleIfNecessary(Entity, NewItem);
        }

        public void Save()
        {
            if(EditItemControl.Visible)
                EditItemControl.Save();

            if (Item != null && pnlReferenced.Visible && !EditItemControl.Visible)
                SexyUncached.TemplateContext.DeleteContentGroupItem(Item.ContentGroupItemID, UserId);

            if (OnSaved != null)
                OnSaved(this, new EventArgs());
        }

        public void Cancel()
        {
            if(EditItemControl != null)
                EditItemControl.Cancel();
        }

        protected void UpdateModuleTitleIfNecessary(ToSic.Eav.Entity entity, ContentGroupItem groupItem)
        {
            // Creating new Context, because EntityTitle gets not refreshed otherwise
            var sexyContext = new SexyContent(ZoneId, AppId, true);

            // Get ContentGroup
            var listContentGroupItem = sexyContext.TemplateContext.GetListContentGroupItem(groupItem.ContentGroupID, UserId);
            var entityModel = sexyContext.ContentContext.GetEntityModel(entity.EntityID);

            // If this is the list title, or no list-title exists, set module title
            if (groupItem.ItemType == ContentGroupItemType.ListContent || (listContentGroupItem == null && groupItem.ItemType == ContentGroupItemType.Content && groupItem.SortOrder == 0) && entityModel.IsPublished)
            {
                var languages = Sexy.ContentContext.GetLanguages();
                // Find Module for default language
                var moduleController = new ModuleController();
                var originalModule = moduleController.GetModule(ModuleID);

                // Update Original Module if no languages active
                if (languages.Count == 0)
                {
                    // Get Title value of Entitiy in current language
                    string titleValue = entityModel.Title[0].ToString();

                    originalModule.ModuleTitle = titleValue;
                    moduleController.UpdateModule(originalModule);
                }

                foreach (var dimension in languages)
                {
                    // Get Title value of Entitiy in current language
                    string titleValue = entityModel.Title[dimension.DimensionID].ToString();

                    if(!originalModule.IsDefaultLanguage)
                        originalModule = originalModule.DefaultLanguageModule;

                    // Break if default language module is null
                    if (originalModule == null)
                        return;

                    // Find module for given Culture
                    var moduleByCulture = moduleController.GetModuleByCulture(originalModule.ModuleID, originalModule.TabID, PortalId, LocaleController.Instance.GetLocale(dimension.ExternalKey));

                    // Break if no module found
                    if (moduleByCulture == null)
                        return;

                    moduleByCulture.ModuleTitle = titleValue;
                    moduleController.UpdateModule(moduleByCulture);
                }
            }
        }

        protected void btnReference_Click(object sender, EventArgs e)
        {
            pnlReferenced.Visible = true;
            btnReference.Visible = false;
            EditItemControl.Visible = false;
            UpdateReferenceChangedHiddenField(true);
        }

        protected void btnClearReference_Click(object sender, EventArgs e)
        {
            EditItemControl.Visible = true;
            pnlReferenced.Visible = false;
            btnReference.Visible = true;
            UpdateReferenceChangedHiddenField(false);
        }

        private void UpdateReferenceChangedHiddenField(bool Referenced)
        {
            if ((Item == null && Referenced) || (Item != null && !Referenced))
                hfReferenceChanged.Value = "false";
            else
                hfReferenceChanged.Value = "true";
        }
    }
}