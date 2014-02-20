using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using System.Dynamic;
using System.Collections.Generic;
using DotNetNuke.Services.Tokens;
using System.Collections.Specialized;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using Microsoft.CSharp;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using ToSic.SexyContent;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        /// <summary>
        /// The Sexy Context
        /// </summary>
        private SexyContent Sexy = new SexyContent();

        #region Private Properties

        /// <summary>
        /// Holds the List of Elements for the current module.
        /// </summary>
        private List<Element> _Elements;
        private List<Element> Elements
        {
            get
            {
                if (_Elements == null)
                {
                    _Elements = Sexy.GetContentElements(ModuleId, Sexy.GetCurrentLanguageName(), null, PortalId).ToList();
                }
                return _Elements;
            }
        }

        private Template _Template;
        private Template Template
        {
            get
            {
                if (!Elements.Any() || !Elements.First().TemplateID.HasValue)
                    return null;
                if(_Template == null)
                    _Template = Sexy.TemplateContext.GetTemplate(Elements.First().TemplateID.Value);
                return _Template;
            }
        }

        private bool IsList
        {
            get
            {
                return Elements.Count > 1;
            }
        }

        private bool UserMayEditThisModule
        {
            get
            {
                return ModuleContext.IsEditable;// ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "edit", ModuleConfiguration);
            }
        }

        private bool StandAlone
        {
            get { return Request.QueryString["standalone"] == "true"; }
        }

        private int? ZoneID
        {
            get { return Sexy.GetZoneID(PortalId); }
        }
        #endregion

        /// <summary>
        /// Page Load event - preload template chooser if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset messages visible states
            pnlMessage.Visible = false;
            pnlError.Visible = false;

            // Add ModuleActionHandler
            AddActionHandler(ModuleActions_Click);

            // If logged in, inject Edit JavaScript, and delete / add items
            if(UserMayEditThisModule)
            {
                ClientScriptManager ClientScript = Page.ClientScript;
                ClientScript.RegisterClientScriptInclude("ViewEdit", ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Js/ViewEdit.js"));

                hfContentGroupItemAction.Visible = true;
                hfContentGroupItemID.Visible = true;
            }

        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // If the Zone (VDB) has not been specified in Portal Settings, show message
            if (!ZoneID.HasValue)
            {
                pnlZoneConfigurationMissing.Visible = true;
                hlkConfigureZone.NavigateUrl = EditUrl(this.TabId, SexyContent.ControlKeys.PortalConfiguration, false,
                                                       "mid", this.ModuleId.ToString());
            }
            else
            {
                // If not fully configured, show stuff
                if (!Elements.Any() || !Elements.First().TemplateID.HasValue || Elements.All(p => !p.EntityID.HasValue))
                    ShowTemplateChooser();

                if (!Sexy.PortalIsConfigured(Server, ControlPath))
                    pnlMissingConfiguration.Visible = true;

                if (Elements.Any() && Elements.First().TemplateID.HasValue)
                    ProcessView();
            }
        }

        /// <summary>
        /// Get the content data and render it with the given template to the page.
        /// </summary>
        protected void ProcessView()
        {
            #region Check if everything has values and return if not

            if (Template == null)
            {
                ShowError(LocalizeString("TemplateConfigurationMissing.Text"));
                return;
            }

            //// ToDo: Fix this
            if (DataSource.GetCache(Sexy.GetZoneID(PortalId).Value).GetContentType(Template.AttributeSetID) == null)
            {
                ShowError("The contents of this module cannot be displayed because it's located in another VDB.");
                return;
            }

            if (Elements.First().Content == null)
            {
                var toolbar = IsEditable ? Sexy.GetElementToolbar(Elements.First().GroupID, Elements.First().SortOrder, Elements.First().ID, ModuleId, LocalResourceFile, false, this, Request.RawUrl) : "";
                ShowMessage(LocalizeString("NoDemoItem.Text") + " " + toolbar);
                return;
            }

            #endregion

            try
            {
                var ListElement = Sexy.GetListElement(ModuleId, Sexy.GetCurrentLanguageName(), null, PortalId);

                // Attach Toolbar to Elements
                Sexy.AttachToolbarToElements(new List<Element> { ListElement }, ModuleId, LocalResourceFile, Template.UseForList, ModuleContext.IsEditable, this, Request.RawUrl);
                Sexy.AttachToolbarToElements(Elements, ModuleId, LocalResourceFile, Template.UseForList, ModuleContext.IsEditable, this, Request.RawUrl);

                string RenderedTemplate = "";

                // Output JSON data if type=data in URL
                if (Request.QueryString["type"] == "data" && Settings.ContainsKey(SexyContent.SettingsPublishDataSource) &&
                    Boolean.Parse(Settings[SexyContent.SettingsPublishDataSource].ToString()))
                {
                    RenderedTemplate = Sexy.GetJsonFromElements(Elements, Sexy.GetCurrentLanguageName());
                    Response.ContentType = "application/json";
                }
                else
                    RenderedTemplate = Template.RenderTemplate(Page, Server, PortalSettings, ModuleContext,
                                                               LocalResourceFile, Elements, ListElement, this,
                                                               Sexy.SexyDataSource);

                // If standalone is specified, output just the template without anything else
                if (StandAlone)
                {
                    Response.Clear();
                    Response.Write(RenderedTemplate);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                    phOutput.Controls.Add(new LiteralControl(RenderedTemplate));
            }
            // Catch errors
            //catch (SexyContentException Ex)
            //{
            //    ShowError(Ex.Message);
            //    Exceptions.LogException(Ex);
            //}
            //catch (System.Web.HttpCompileException Ex)
            //{
            //    ShowError(Ex.Message);
            //    Exceptions.LogException(Ex);
            //}
            // catch all other errors
            catch (Exception Ex)
            {
                //Exceptions.ProcessModuleLoadException(this, Ex);
                ShowError(LocalizeString("TemplateError.Text") + ": " + HttpUtility.HtmlEncode(Ex.ToString()), LocalizeString("TemplateError.Text"), false);
                Exceptions.LogException(Ex);
            }
        }

        #region Show Message or Errors
        protected void ShowMessage(string Message, bool ShowOnlyEdit = true)
        {
            if (!ShowOnlyEdit || UserMayEditThisModule)
            {
                pnlMessage.Controls.Add(new LiteralControl(Message));
                pnlMessage.Visible = true;
            }
        }

        protected void ShowError(string Error, string NonHostError = "", bool ShowOnlyEdit = true)
        {
            if (String.IsNullOrEmpty(NonHostError))
                NonHostError = Error;

            if (!ShowOnlyEdit || UserMayEditThisModule)
            {
                pnlError.Controls.Add(new LiteralControl(UserInfo.IsSuperUser ? Error : NonHostError));
                pnlError.Visible = true;
            }
        }
        #endregion

        /// <summary>
        /// Show the Template Chooser directly on the page, but only if the user has edit rights
        /// </summary>
        protected void ShowTemplateChooser()
        {
            // If there are no templates configured
            if (!Sexy.TemplateContext.GetVisibleTemplates(PortalId).Any())
            {
                pnlGetStarted.Visible = true;
            }
            else
            {
                if (!Page.IsPostBack && UserMayEditThisModule)
                {
                    pnlTemplateChooser.Visible = true;

                    ddlContentType.DataSource = Sexy.GetAvailableAttributeSetsForVisibleTemplates(PortalId);
                    ddlContentType.DataBind();

                    if (Template != null)
                        ddlContentType.SelectedValue = Template.AttributeSetID.ToString();

                    BindTemplateDropDown();
                }

                if (IsList)
                    ddlContentType.Enabled = false;
            }

        }

        protected void BindTemplateDropDown()
        {
            IEnumerable<Template> TemplatesToChoose;

            if (!IsList)
                TemplatesToChoose = Sexy.TemplateContext.GetVisibleTemplates(PortalId);
            else
                TemplatesToChoose = Sexy.TemplateContext.GetVisibleListTemplates(PortalId);

            // Make only templates from chosen content type shown if content type is set
            if (ddlContentType.SelectedValue != "0")
                TemplatesToChoose = TemplatesToChoose.Where(p => p.AttributeSetID == int.Parse(ddlContentType.SelectedValue));

            ddlTemplate.DataSource = TemplatesToChoose;
            ddlTemplate.DataBind();
            // If the current data is a list of entities, don't allow changing back to no template
            if(!IsList)
                ddlTemplate.Items.Insert(0, new ListItem(LocalizeString("ddlTemplateDefaultItem.Text"), "0"));

            // If there are elements and the selected template exists in the list, select that
            if (Elements.Any() && ddlTemplate.Items.FindByValue(Elements.First().TemplateID.ToString()) != null)
                ddlTemplate.SelectedValue = Elements.First().TemplateID.ToString();
        }

        protected void ChangeTemplate()
        {
            if (UserMayEditThisModule)
            {
                var TemplateID = int.Parse(ddlTemplate.SelectedValue);

                // Return if current TemplateID is already set
                if (Template != null && TemplateID == Template.TemplateID)
                    return;

                new SexyContent(false).UpdateTemplateForGroup(Sexy.GetContentGroupIDFromModule(ModuleId), TemplateID,
                                                              UserId);

                Response.Redirect(Request.RawUrl);
            }
        }

        /// <summary>
        /// Set the chosen template and redirect to the current page (in order to show the chosen template on the page)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTemplate();
        }

        protected void ddlContentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTemplateDropDown();

            if (ddlContentType.SelectedValue != "0")
            {
                if(ddlTemplate.Items.Count > 1)
                    ddlTemplate.SelectedIndex = 1;

                ChangeTemplate();
            }
        }

        /// <summary>
        /// Value changed handler for hfContentGroupItemAction. Allows deleting / adding ContentGroupItems from inside the template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void hfContentGroupItemAction_ValueChanged(object sender, EventArgs e)
        {
            if (UserMayEditThisModule)
            {
                switch (hfContentGroupItemAction.Value)
                {
                    case "add":
                        new SexyContent(false).AddContentGroupItem(Elements.First().GroupID, UserId, Elements.First().TemplateID, null, Elements.Where(el => el.ID == int.Parse(hfContentGroupItemID.Value)).Single().SortOrder + 1, true, ContentGroupItemType.Content, false);
                        Response.Redirect(Request.RawUrl);
                        break;
                }  

                hfContentGroupItemAction.Value = "";
                hfContentGroupItemID.Value = "";
            }
        }

        #region ModuleActions
        /// <summary>
        /// Causes DNN to create the menu with all actions like edit entity, new, etc.
        /// </summary>
        private DotNetNuke.Entities.Modules.Actions.ModuleActionCollection _ModuleActions;
        public DotNetNuke.Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                if (_ModuleActions == null)
                {
                    _ModuleActions = new DotNetNuke.Entities.Modules.Actions.ModuleActionCollection();
                    DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Actions = _ModuleActions;

                    if (!IsList)
                    {
                        if (Elements.Any() && Elements.First().TemplateID.HasValue)
                        {
                            Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), DotNetNuke.Entities.Modules.Actions.ModuleActionType.EditContent, "edititem", "edit.gif", UrlUtils.PopUpUrl(Sexy.GetElementEditLink(Elements.First().GroupID, Elements.First().SortOrder, ModuleId, TabId, ""), this, PortalSettings, false, false), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                        }
                    }
                    else
                    {
                        // Edit List
                        Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"), DotNetNuke.Entities.Modules.Actions.ModuleActionType.ContentOptions, "editlist", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditList, false, "mid", this.ModuleId.ToString(), SexyContent.ContentGroupIDString, Elements.First().GroupID.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                    }

                    if (Elements.Any() && Elements.First().TemplateID.HasValue && Template != null && Template.UseForList)
                    {
                        // Add Item (this action will do a postback)
                        Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), SexyContent.ControlKeys.AddItem, SexyContent.ControlKeys.AddItem, "add.gif", "", true, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                    }

                    // Settings Button
                    if (Sexy.TemplateContext.GetVisibleTemplates(PortalSettings.PortalId).Any())
                    {
                        Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), DotNetNuke.Entities.Modules.Actions.ModuleActionType.AddContent, "settings", "action_settings.gif", EditUrl(SexyContent.ControlKeys.SettingsWrapper), false, SecurityAccessLevel.Edit, true, false); }

                    if (!Sexy.SexyContentDesignersGroupConfigured(PortalId) || Sexy.IsInSexyContentDesignersGroup(UserInfo))
                    {
                        if (Template != null)
                        {
                            // Edit Template Button
                            Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), DotNetNuke.Entities.Modules.Actions.ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", this.ModuleId.ToString(), "TemplateID", Template.TemplateID.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, true);
                        }

                        // Manage Templates Button
                        //Actions.Add(GetNextActionID(), LocalizeString("ActionManageTemplates.Text"), "ManageTemplates.Action", "managetemplates", "action_settings.gif", EditUrl(SexyContent.ControlKeys.ManageTemplates), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, false);

                        // Export Button
                        // Hide this button temporarily (will be vibsible in a future version)
                        // Actions.Add(GetNextActionID(), LocalizeString("ActionExport.Text"), DotNetNuke.Entities.Modules.Actions.ModuleActionType.ExportModule, "export", "action_export.gif", EditUrl(SexyContent.ControlKeys.Export), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, false);

                        // Manage EAV Button
                        //Actions.Add(GetNextActionID(), LocalizeString("ActionManageContentTypes.Text"), "ManageContentTypes.Action", "eavmanagement", "action_settings.gif", EditUrl("", "", SexyContent.ControlKeys.EavManagement, new string[] { "CultureDimension", Sexy.GetCurrentLanguageID().ToString() }), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, false);

                        // Administrator functions
                        Actions.Add(GetNextActionID(), "Admin", "Admin.Action",
                                    "gettingstarted", "action_settings.gif", EditUrl("", "", "gettingstarted"),
                                    false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, false);
                    }
                }
                return _ModuleActions;
            }
        }

        protected void ModuleActions_Click(object sender, DotNetNuke.Entities.Modules.Actions.ActionEventArgs e)
        {
            switch (e.Action.CommandName)
            {
                case SexyContent.ControlKeys.AddItem:
                    new SexyContent(false).AddContentGroupItem(Elements.First().GroupID, UserId, Template.TemplateID, null, null, true, ContentGroupItemType.Content, false);
                    Response.Redirect(Request.RawUrl);
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Configure Portal (creates /Portals/[Directory]/2sexy folder with web.config for Razor templates)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void hlkConfigurePortal_Click(object sender, EventArgs e)
        {
            if (!Sexy.PortalIsConfigured(Server, ControlPath))
            {
                Sexy.ConfigurePortal(Server);
                Response.Redirect(Request.RawUrl);
            }
        }
    }
}