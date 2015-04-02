using DotNetNuke.Entities.Content;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

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
                    {
                        return sortOrder;
                    }
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
        public Guid? ContentGroupID
        {
            get
            {
                string ContentGroupIDString = Request.QueryString["ContentGroupID"];

                if (!String.IsNullOrEmpty(ContentGroupIDString))
                    return Guid.Parse(ContentGroupIDString);

                return null;
            }
        }

		private ContentGroup _contentGroup;
		private ContentGroup ContentGroup
		{
			get
			{
				if (_contentGroup == null)
					_contentGroup = Sexy.ContentGroups.GetContentGroup(ContentGroupID.Value);
				return _contentGroup;
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
            var languages = Sexy.ContentContext.GetLanguages().Where(l => l.Active).OrderByDescending(l => l.DimensionID == DefaultLanguageID).ThenBy(l => l.ExternalKey);
            if (!languages.Any())
                pnlDimensionNav.Visible = false;
            rptDimensions.DataSource = languages;
            rptDimensions.DataBind();

            btnDelete.OnClientClick = "return confirm('" + LocalizeString("btnDelete.Confirm") + "')";
            btnDelete.Text = ContentGroup.Content.Count > 1 ? LocalizeString("btnDelete.ListText") : LocalizeString("btnDelete.Text");
            btnDelete.Visible = !NewMode && ContentGroupID.HasValue;

			// If there is something to edit
			if (SortOrder.HasValue && (SortOrder == -1 || SortOrder < ContentGroup.Content.Count))
			{
				// Settings link (to change content)
				hlkChangeContent.NavigateUrl = Sexy.GetElementSettingsLink(ContentGroupID.Value, SortOrder.Value, ModuleId, TabId, Request.RawUrl);

				// Show Change Content or Reference Link only if this is the default language
				var isDefaultLanguage = LanguageID == DefaultLanguageID;
				hlkChangeContent.Visible = !NewMode && isDefaultLanguage;
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
			if (ContentGroupID.HasValue && SortOrder.HasValue)
			{
				var types = new List<string>();
				if (SortOrder == -1)
					types = new List<string>() {"ListContent", "ListPresentation"};
				else
					types = new List<string>() { "Content", "Presentation" };

				foreach (var type in types)
				{
					var contentTypeStaticName = ContentGroup.Template.ContentTypeStaticName;
					if (type == "Presentation")
						contentTypeStaticName = ContentGroup.Template.PresentationTypeStaticName;
					if (type == "ListContent")
						contentTypeStaticName = ContentGroup.Template.ListContentTypeStaticName;
					if (type == "ListPresentation")
						contentTypeStaticName = ContentGroup.Template.ListPresentationTypeStaticName;

					if (String.IsNullOrEmpty(contentTypeStaticName))
						continue;

					var editControl =
						(EditContentGroupItem) LoadControl(System.IO.Path.Combine(TemplateSourceDirectory, "EditContentGroupItem.ascx"));
					editControl.ContentGroupID = ContentGroupID.Value;
					editControl.AppId = AppId.Value;
					editControl.ZoneId = ZoneId.Value;
					editControl.TemplateID = ContentGroup.Template.TemplateId;
					editControl.AttributeSetStaticName = contentTypeStaticName;
					editControl.ItemType = type;
					editControl.SortOrder = SortOrder;
					editControl.ModuleID = ModuleId;
					editControl.TabID = TabId;
					editControl.NewMode = NewMode;
					phNewOrEditControls.Controls.Add(editControl);
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
			ContentGroup.RemoveContentAndPresentationEntities(SortOrder.Value);
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
            Url = Regex.Replace(Url, "(&|/)CultureDimension(=|/)[0-9]+", "", RegexOptions.IgnoreCase);
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