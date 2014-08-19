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
    public partial class EditEntity : PortalModuleBase, IEditContentControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the EntityId
        /// </summary>
        public int? EntityId { get; set; }

        public bool IsPublished
        {
            get { return EditItemControl.IsPublished; }
            set { EditItemControl.IsPublished = value; }
        }

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

        public int? KeyNumber
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["KeyNumber"])
                    ? int.Parse(Request.QueryString["KeyNumber"])
                    : new int?();
            }
        }

        public int AssignmentObjectTypeId
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["AssignmentObjectTypeId"])
                    ? int.Parse(Request.QueryString["AssignmentObjectTypeId"])
                    : SexyContent.AssignmentObjectTypeIDDefault;
            }
        }

        #endregion

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
            LocalResourceFile = LocalResourceFile + "EditEntity.ascx";

            ProcessView();

            hSectionHead.ID = "SexyContent-EditSection-Entity";

            if (IsPostBack)
                return;

            lblNewOrEditItemHeading.Text = "Entity";
        }

        protected void ProcessView()
        {
            EditItemControl = (ItemForm)LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "SexyContent/EAV/Controls/ItemForm.ascx"));
            EditItemControl.DefaultCultureDimension = DefaultLanguageID != 0 ? DefaultLanguageID : new int?();
            EditItemControl.IsDialog = false;
            EditItemControl.HideNavigationButtons = true;
            EditItemControl.PreventRedirect = true;
            EditItemControl.AttributeSetId = AttributeSetID;
            EditItemControl.AssignmentObjectTypeId = AssignmentObjectTypeId;
            EditItemControl.KeyNumber = KeyNumber;
            EditItemControl.ZoneId = ZoneId;
            EditItemControl.AppId = AppId;
	        EditItemControl.AddClientScriptAndCss = false;
            EditItemControl.ItemHistoryUrl = "";
            EditItemControl.PreventRedirect = Request.QueryString["PreventRedirect"] == "true";

            var newItemUrl = EditUrl(this.TabID, SexyContent.ControlKeys.EditContentGroup, true, new string[] { });
            EditItemControl.NewItemUrl = newItemUrl + (newItemUrl.Contains("?") ? "&" : "?") + "AppID=" + AppId.ToString() + "&mid=" + ModuleID.ToString() + "&AttributeSetId=[AttributeSetId]&EditMode=New&CultureDimension=" + this.LanguageID;

            // If ContentGroupItem has Entity, edit that; else create new Entity
            if (EntityId.HasValue)
            {
                EditItemControl.Updated += EditItem_OnEdited;
                EditItemControl.EntityId = EntityId.Value;
                EditItemControl.InitForm(FormViewMode.Edit);

                hlkHistory.Visible = true;
                hlkHistory.NavigateUrl = EditUrl("", "", SexyContent.ControlKeys.EavManagement, new string[] { "AppID", AppId.ToString(), "ManagementMode", "ItemHistory", "EntityId", EntityId.Value.ToString(), "mid", ModuleID.ToString() });
            }
            // Create a new Entity
            else
            {
                EditItemControl.Inserted += NewItem_OnInserted;
                EditItemControl.Visible = true;
                EditItemControl.InitForm(FormViewMode.Insert);
            }

            phNewOrEditItem.Controls.Add(EditItemControl);
        }

        protected void EditItem_OnEdited(ToSic.Eav.Entity Entity)
        {
        }

        protected void NewItem_OnInserted(ToSic.Eav.Entity Entity)
        {
        }

        public void Save()
        {
            if(EditItemControl.Visible)
                EditItemControl.Save();

            if (OnSaved != null)
                OnSaved(this, new EventArgs());
        }

        public void Cancel()
        {
            if(EditItemControl != null)
                EditItemControl.Cancel();
        }
    }
}