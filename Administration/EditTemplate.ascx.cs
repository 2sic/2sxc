using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using ToSic.Eav;
using ToSic.Eav.DataSources;
using ToSic.SexyContent.EAV.PipelineDesigner;

namespace ToSic.SexyContent
{
    public partial class EditTemplate : SexyControlAdminBase
    {

        private bool ModeIsEdit { get { return !String.IsNullOrEmpty(Request.QueryString[SexyContent.TemplateID]); } }
        private int TemplateID { get { return Convert.ToInt32(Request.QueryString[SexyContent.TemplateID]); } }
        private Template Template;

        /// <summary>
        /// Initialize the template if edit mode is active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (ModeIsEdit)
                Template = SexyUncached.Templates.GetTemplate(TemplateID);

            var contentTypeSelectors = new[] { ctrContentType, ctrPresentationType, ctrListContentType, ctrListPresentationType };

            foreach (var contentTypeSelector in contentTypeSelectors)
            {
                contentTypeSelector.AppId = AppId.Value;
                contentTypeSelector.ZoneId = ZoneId.Value;
            }

            if (!IsContentApp)
                ddlTemplateLocations.Enabled = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            lblPublishSource.HelpText = LocalizeString("lblPublishSource.Help").Replace("[JSONTestLink]", GetJsonUrl());
            lblPublishSource.Text = LocalizeString("lblPublishSource.Text");
            lblPublishSource.ResourceKey = "-";

        }

        protected string GetJsonUrl()
        {
            var url = DotNetNuke.Common.Globals.NavigateURL(this.TabId);
            url += (url.Contains("?") ? "&" : "?") + "mid=" + ModuleId.ToString() +
                   "&standalone=true&type=data&popUp=true";
            return url;
        }

        /// <summary>
        /// Set values in the form if edit mode is active
        /// Set DotNetNuke modal window NavigateUrl for cancel link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // set DotNetNuke modal window Url for cancel link
            hlkCancel.NavigateUrl = EditUrl(PortalSettings.ActiveTab.TabID, SexyContent.ControlKeys.ManageTemplates, true, "mid=" + ModuleId + "&" + SexyContent.AppIDString + "=" + AppId);
	        hlkManagePipelines.NavigateUrl = PipelineManagementDnnWrapper.GetEditUrl(this, AppId.Value);

            InitializeForm();

            // Set localized Error Messages for Validators
            valTemplateName.ErrorMessage = LocalizeString("valTemplateName.ErrorMessage");
            valTemplateFile.ErrorMessage = LocalizeString("valTemplateFile.ErrorMessage");
            valTemplateFileName.ErrorMessage = LocalizeString("valTemplateFileName.ErrorMessage");

            pnlSeparateContentPresentation.Visible = chkSeparateContentPresentation.Checked;

			// show some fields only for App-Module
	        if (IsContentApp)
	        {
		        pnlDataPipeline.Visible = false;
		        pnlViewNameInUrl.Visible = false;
	        }
        }

        /// <summary>
        /// Set values in the form if edit mode is active, or fill available values
        /// </summary>
        protected void InitializeForm()
        {

            if (Page.IsPostBack)
            {
                pnlListConfiguration.Visible = chkEnableList.Checked;
                var isNoContentType = (ctrContentType.ContentTypeID == -1);
                if (isNoContentType)
                {
                    chkEnableList.Checked = false;
                    ctrPresentationType.ContentTypeID = 0;
                    chkSeparateContentPresentation.Checked = false;
                }
                chkEnableList.Enabled = !isNoContentType;
                chkSeparateContentPresentation.Enabled = !isNoContentType;
                ctrPresentationType.Enabled = !isNoContentType;
                return;
            }

            // DataBind Template Locations
            ddlTemplateLocations.Items.Add(new ListItem(LocalizeString("TemplateLocationPortalFileSystem.Text"), SexyContent.TemplateLocations.PortalFileSystem));
            ddlTemplateLocations.Items.Add(new ListItem(LocalizeString("TemplateLocationHostFileSystem.Text"), SexyContent.TemplateLocations.HostFileSystem));

            txtPublishStreams.Text = "Default,ListContent";

            // Fill form with values if in edit mode
            if (ModeIsEdit)
            {
                txtTemplateName.Text = Template.Name;
                ddlTemplateLocations.SelectedValue = Template.Location;
                ddlTemplateTypes.SelectedValue = Template.Type;
                chkHidden.Checked = Template.IsHidden;
                chkEnableList.Checked = Template.UseForList;
                pnlListConfiguration.Visible = chkEnableList.Checked;
                txtPublishStreams.Text = Template.StreamsToPublish;
                chkPublishSource.Checked = Template.PublishData;
	            ddlDataPipeline.SelectedValue = (Template.Pipeline != null ? Template.Pipeline.EntityId : 0).ToString();
	            txtViewNameInUrl.Text = Template.ViewNameInUrl;

                // Set ContentType / Demo Entity Selectors
                SetTemplateDefaultSelector(Template.TemplateId, ctrContentType);
                SetTemplateDefaultSelector(Template.TemplateId, ctrPresentationType);
                SetTemplateDefaultSelector(Template.TemplateId, ctrListContentType);
                SetTemplateDefaultSelector(Template.TemplateId, ctrListPresentationType);

                chkSeparateContentPresentation.Checked = pnlSeparateContentPresentation.Visible = ctrPresentationType._ContentTypeID > 0;
            }

            // Bind template files dropdown
            BindTemplateFiles(ddlTemplateTypes.SelectedValue, ddlTemplateLocations.SelectedValue, ddlTemplateFiles);

			// Bind DataPipeline DropDown
	        BindDataPipelineDropDown();

            // Set value of demo-content and template file dropdown if in edit mode
            if (ModeIsEdit)
            {
                if (ddlTemplateFiles.Items.Cast<ListItem>().Any(l => l.Value == Template.Path))
                    ddlTemplateFiles.SelectedValue = Template.Path;
                if (ddlTemplateFiles.Items.Cast<ListItem>().Any(l => l.Value == Template.Path.Replace('\\','/')))
                    ddlTemplateFiles.SelectedValue = Template.Path.Replace('\\', '/');
            }
        }

        /// <summary>
        /// After the Update button is clicked, updates the template or creates a new one,
        /// depending if in edit mode or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdate_Click(object sender, EventArgs e)
        {

			// ToDo: Fix template update
			throw new NotImplementedException("ToDo: Fix Template Update");

			//var attributeSetId = ctrContentType.ContentTypeID.HasValue && ctrContentType.ContentTypeID > 0 ? ctrContentType.ContentTypeID.Value : new int?();

			//// Get a new template if the temlpate does not exist yet, else take existing
			//Template = ModeIsEdit ? Template : Sexy.Templates.GetNewTemplate(AppId.Value);

			//Template.PortalID = this.PortalId;
			//	Template.AttributeSetID = attributeSetId;
			//	Template.DemoEntityID = ctrContentType.DemoEntityID;
			//	Template.Location = ddlTemplateLocations.SelectedValue;
			//	Template.Type = ddlTemplateTypes.SelectedValue;
			//	Template.PipelineEntityID = ddlDataPipeline.SelectedValue == "0" ? (int?)null : int.Parse(ddlDataPipeline.SelectedValue);
			//	Template.ViewNameInUrl = txtViewNameInUrl.Text;
			//	Template.SysModifiedBy = UserId;
			//	Template.SysModified = DateTime.Now;
			//Template.Name = txtTemplateName.Text;
			//	Template.Script = "";
			//	Template.IsHidden = chkHidden.Checked;
			//	Template.UseForList = chkEnableList.Checked;
			//	Template.AppID = AppId.Value;
			//Template.PublishData = chkPublishSource.Checked;
			//Template.StreamsToPublish = txtPublishStreams.Text;

			//if (pnlSelectTemplateFile.Visible)
			//	Template.Path = ddlTemplateFiles.SelectedValue;
			//else
			//	SexyUncached.CreateTemplateFileIfNotExists(txtTemplateFileName.Text, Template, Server, LocalizeString("NewTemplateFile.DefaultText"));

			//if (ModeIsEdit)
			//{
			//	SexyUncached.Templates.UpdateTemplate(Template);
			//}
			//else
			//{
			//	Template.SysCreatedBy = UserId;
			//	SexyUncached.Templates.AddTemplate(Template);
			//}

			//if (!chkSeparateContentPresentation.Checked)
			//	ctrPresentationType.ContentTypeID = new int?();

			//// Add template configuration entities for presentation, list header content type, list content, etc.    
			//SexyUncached.CreateOrUpdateTemplateDefault(Template.TemplateID, ContentGroupItemType.Presentation.ToString("F"), ctrPresentationType.ContentTypeID, ctrPresentationType.DemoEntityID);
			//SexyUncached.CreateOrUpdateTemplateDefault(Template.TemplateID, ContentGroupItemType.ListContent.ToString("F"), ctrListContentType.ContentTypeID, ctrListContentType.DemoEntityID);
			//SexyUncached.CreateOrUpdateTemplateDefault(Template.TemplateID, ContentGroupItemType.ListPresentation.ToString("F"), ctrListPresentationType.ContentTypeID, ctrListPresentationType.DemoEntityID);

			//// Redirect to the manage templates control
			//string RedirectUrl = UrlUtils.PopUpUrl(DotNetNuke.Common.Globals.NavigateURL(SexyContent.ControlKeys.ManageTemplates, "mid", ModuleId.ToString(), SexyContent.AppIDString, AppId.ToString()), this, PortalSettings, false, true);
			//Response.Redirect(RedirectUrl);
        }

        protected void SetTemplateDefaultSelector(int TemplateID, ContentTypeAndDemoSelector Selector)
        {
			// ToDo: Fix template default selector...
			throw new NotImplementedException("Fix Template default selector!");

			//var ItemType = Selector.ItemType;
			//var TemplateDefault = Sexy.GetTemplateDefault(TemplateID, ItemType);

			//if (TemplateDefault != null)
			//{
			//	Selector.ContentTypeID = TemplateDefault.ContentTypeID;
			//	Selector.DemoEntityID = TemplateDefault.DemoEntityID;
			//}

			//Selector.Enabled = !Sexy.IsTemplateDefaultInUse(TemplateID, ItemType);
        }

        protected void btnCreateTemplateFile_Click(object sender, EventArgs e)
        {
            pnlSelectTemplateFile.Visible = false;
            valTemplateFile.Enabled = false;
            pnlCreateTemplateFile.Visible = true;
            valTemplateFileName.Enabled = true;

            if(!UserInfo.IsSuperUser)
                ddlTemplateLocations.Items.Remove(ddlTemplateLocations.Items.FindByValue(SexyContent.TemplateLocations.HostFileSystem));
            
            string ProposedTemplateFile = "";
            if(ddlTemplateTypes.SelectedValue == "C# Razor" || ddlTemplateTypes.SelectedValue == "VB Razor")
                ProposedTemplateFile += "_";
            
            ProposedTemplateFile += txtTemplateName.Text;

            switch (ddlTemplateTypes.SelectedValue)
            {
                case "C# Razor":
                    ProposedTemplateFile += ".cshtml";
                    break;
                case "VB Razor":
                    ProposedTemplateFile += ".vbhtml";
                    break;
                default:
                    ProposedTemplateFile += ".html";
                    break;
            }

            txtTemplateFileName.Text = ProposedTemplateFile;
        }

        /// <summary>
        /// Bind the content types if the template location dropdown changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTemplateLocations_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTemplateFiles(ddlTemplateTypes.SelectedValue, ddlTemplateLocations.SelectedValue, ddlTemplateFiles);
        }

        /// <summary>
        /// Bind template files if the template type changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTemplateTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTemplateFiles(ddlTemplateTypes.SelectedValue, ddlTemplateLocations.SelectedValue, ddlTemplateFiles);
        }

        /// <summary>
        /// Bind the template files from the given template location
        /// </summary>
        /// <param name="TemplateType">The template type</param>
        /// <param name="TemplateLocation">The template location</param>
        /// <param name="TemplateDropDown">The template dropdown to databind</param>
        private void BindTemplateFiles(string TemplateType, string TemplateLocation, DropDownList TemplateDropDown)
        {
            TemplateDropDown.DataSource = Sexy.GetTemplateFiles(Server, TemplateType, TemplateLocation);
            TemplateDropDown.DataBind();
        }

		/// <summary>
		/// Bind the DataPipeline DropDown with Pipelines for this App
		/// </summary>
	    private void BindDataPipelineDropDown()
	    {
		    var source = DataSource.GetInitialDataSource(ZoneId, AppId);
		    var typeFilter = DataSource.GetDataSource<EntityTypeFilter>(ZoneId, AppId, source);
		    typeFilter.TypeName = DataSource.DataPipelineStaticName;

		    ddlDataPipeline.DataSource = typeFilter.List.Select(e => new
			{
				PipelineEntityID = e.Key,
				Name = string.Format("{0} ({1})", ((ToSic.Eav.Data.Attribute<string>)e.Value["Name"]).TypedContents, e.Key)
			}).OrderBy(e => e.Name);
		    ddlDataPipeline.DataBind();
	    }
    }
}