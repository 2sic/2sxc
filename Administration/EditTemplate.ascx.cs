using System;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using ToSic.Eav;
using ToSic.Eav.Data;
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
                Template = Sexy.Templates.GetTemplate(TemplateID);

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
            var url = Globals.NavigateURL(TabId);
            url += (url.Contains("?") ? "&" : "?") + "mid=" + ModuleId +
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
	        hlkManagePipelines.NavigateUrl =  ToSic.SexyContent.EAV.PipelineDesigner.PipelineManagementDnnWrapper.GetEditUrl(this, AppId.Value);

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
                var isNoContentType = String.IsNullOrEmpty(ctrContentType.ContentTypeStaticName);
                if (isNoContentType)
                {
                    chkEnableList.Checked = false;
                    ctrPresentationType.ContentTypeStaticName = "";
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

                chkSeparateContentPresentation.Checked = pnlSeparateContentPresentation.Visible = !String.IsNullOrEmpty(ctrPresentationType._ContentTypeStaticName);
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
	        var templatePath = ddlTemplateFiles.SelectedValue;

			if (!pnlSelectTemplateFile.Visible)
				templatePath = Sexy.CreateTemplateFileIfNotExists(txtTemplateFileName.Text, ddlTemplateTypes.SelectedValue, ddlTemplateLocations.SelectedValue, Server, LocalizeString("NewTemplateFile.DefaultText"));

	        var templateId = ModeIsEdit ? Template.TemplateId : new int?();
	        var pipelineEntityId = ddlDataPipeline.SelectedValue == "0" ? (int?) null : int.Parse(ddlDataPipeline.SelectedValue);

			if (!chkSeparateContentPresentation.Checked)
				ctrPresentationType.ContentTypeStaticName = "";

			Sexy.Templates.UpdateTemplate(templateId, txtTemplateName.Text, templatePath, ctrContentType.ContentTypeStaticName, ctrContentType.DemoEntityID, ctrPresentationType.ContentTypeStaticName, ctrPresentationType.DemoEntityID, ctrListContentType.ContentTypeStaticName, ctrListContentType.DemoEntityID, ctrListPresentationType.ContentTypeStaticName, ctrListPresentationType.DemoEntityID, ddlTemplateTypes.SelectedValue, chkHidden.Checked, ddlTemplateLocations.SelectedValue, chkEnableList.Checked, chkPublishSource.Checked, txtPublishStreams.Text, pipelineEntityId, txtViewNameInUrl.Text);

			// Redirect to the manage templates control
			var RedirectUrl = UrlUtils.PopUpUrl(Globals.NavigateURL(SexyContent.ControlKeys.ManageTemplates, "mid", ModuleId.ToString(), SexyContent.AppIDString, AppId.ToString()), this, PortalSettings, false, true);
			Response.Redirect(RedirectUrl);

        }

		protected void SetTemplateDefaultSelector(int TemplateID, ContentTypeAndDemoSelector Selector)
		{
			var itemType = Selector.ItemType;
			var template = Sexy.Templates.GetTemplate(TemplateID);

			if (itemType == "Content")
			{
				Selector.ContentTypeStaticName = template.ContentTypeStaticName;
				Selector.DemoEntityID = template.ContentDemoEntity != null ? template.ContentDemoEntity.EntityId : new int?();
			}

			if (itemType == "Presentation")
			{
				Selector.ContentTypeStaticName = template.PresentationTypeStaticName;
				Selector.DemoEntityID = template.PresentationDemoEntity != null ? template.PresentationDemoEntity.EntityId : new int?();
			}

			if (itemType == "ListContent")
			{
				Selector.ContentTypeStaticName = template.ListContentTypeStaticName;
				Selector.DemoEntityID = template.ListContentDemoEntity != null ? template.ListContentDemoEntity.EntityId : new int?();
			}

			if (itemType == "ListPresentation")
			{
				Selector.ContentTypeStaticName = template.ListPresentationTypeStaticName;
				Selector.DemoEntityID = template.ListPresentationDemoEntity != null ? template.ListPresentationDemoEntity.EntityId : new int?();
			}

			Selector.Enabled = !Sexy.ContentGroups.IsConfigurationInUse(TemplateID, itemType);
		}

        protected void btnCreateTemplateFile_Click(object sender, EventArgs e)
        {
            pnlSelectTemplateFile.Visible = false;
            valTemplateFile.Enabled = false;
            pnlCreateTemplateFile.Visible = true;
            valTemplateFileName.Enabled = true;

            if(!UserInfo.IsSuperUser)
                ddlTemplateLocations.Items.Remove(ddlTemplateLocations.Items.FindByValue(SexyContent.TemplateLocations.HostFileSystem));
            
            var ProposedTemplateFile = "";
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
		    typeFilter.TypeName = Constants.DataPipelineStaticName;

		    ddlDataPipeline.DataSource = typeFilter.List.Select(e => new
			{
				PipelineEntityID = e.Key,
				Name = string.Format("{0} ({1})", ((Attribute<string>)e.Value["Name"]).TypedContents, e.Key)
			}).OrderBy(e => e.Name);
		    ddlDataPipeline.DataBind();
	    }
    }
}