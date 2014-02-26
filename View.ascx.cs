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
using DotNetNuke.Entities.Modules.Actions;

namespace ToSic.SexyContent
{
    public partial class View : SexyControlEditBase, IActionable
    {

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
                if (!Elements.Any() || !Elements.First().TemplateId.HasValue)
                    return null;
                if(_Template == null)
                    _Template = Sexy.TemplateContext.GetTemplate(Elements.First().TemplateId.Value);
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

        #region Private Properties

        private bool UserMayEditThisModule
        {
            get
            {
                return ModuleContext.IsEditable;
            }
        }

        private bool StandAlone
        {
            get { return Request.QueryString["standalone"] == "true"; }
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
            if (!ZoneId.HasValue)
            {
                pnlZoneConfigurationMissing.Visible = true;
                hlkConfigureZone.NavigateUrl = EditUrl(this.TabId, SexyContent.ControlKeys.PortalConfiguration, false,
                                                       "mid", this.ModuleId.ToString(), SexyContent.AppIDString + "=" + AppId);
            }
            else
            {
                // If not fully configured, show stuff
                if (!Elements.Any() || !Elements.First().TemplateId.HasValue || Elements.All(p => !p.EntityId.HasValue))
                    ShowTemplateChooser();

                if (!Sexy.PortalIsConfigured(Server, ControlPath))
                    pnlMissingConfiguration.Visible = true;

                if (Elements.Any() && Elements.First().TemplateId.HasValue)
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

            if (DataSource.GetCache(ZoneId.Value).GetContentType(Template.AttributeSetID) == null)
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
                                                               Sexy.SexyDataSource, Sexy, AppId.Value);

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
            var noTemplatesYet = !Sexy.GetVisibleTemplates(PortalId).Any();

            // If there are no templates configured
            if (noTemplatesYet)
            {
                pnlGetStarted.Visible = true;
            }

            if(!noTemplatesYet || !IsContentApp)
            {
                if (!Page.IsPostBack && UserMayEditThisModule)
                {
                    pnlTemplateChooser.Visible = true;

                    ddlContentType.DataSource = Sexy.GetAvailableAttributeSetsForVisibleTemplates(PortalId);
                    ddlContentType.DataBind();

                    if (Template != null)
                        ddlContentType.SelectedValue = Template.AttributeSetID.ToString();

                    BindTemplateDropDown();

                    if (!IsContentApp)
                    {
                        ddlApp.Visible = !IsContentApp;
                        BindAppDropDown();
                    }
                    
                }

                if (IsList)
                    ddlContentType.Enabled = false;
            }

            ddlContentType.Visible = IsContentApp;
            ddlTemplate.Enabled = IsContentApp || AppId.HasValue;
        }

        protected void BindTemplateDropDown()
        {
            IEnumerable<Template> TemplatesToChoose;

            if (!IsList)
                TemplatesToChoose = Sexy.GetVisibleTemplates(PortalId);
            else
                TemplatesToChoose = Sexy.GetVisibleListTemplates(PortalId);

            // Make only templates from chosen content type shown if content type is set
            if (ddlContentType.SelectedValue != "0")
                TemplatesToChoose = TemplatesToChoose.Where(p => p.AttributeSetID == int.Parse(ddlContentType.SelectedValue));

            ddlTemplate.DataSource = TemplatesToChoose;
            ddlTemplate.DataBind();
            // If the current data is a list of entities, don't allow changing back to no template
            if(!IsList)
                ddlTemplate.Items.Insert(0, new ListItem(LocalizeString("ddlTemplateDefaultItem.Text"), "0"));

            // If there are elements and the selected template exists in the list, select that
            if (Elements.Any() && ddlTemplate.Items.FindByValue(Elements.First().TemplateId.ToString()) != null)
                ddlTemplate.SelectedValue = Elements.First().TemplateId.ToString();
            else
            {
                if (ddlContentType.SelectedValue != "0")
                {
                    if (ddlTemplate.Items.Count > 1)
                        ddlTemplate.SelectedIndex = 1;

                    ChangeTemplate();
                }
            }
        }

        private void BindAppDropDown()
        {
            ddlApp.DataSource = Sexy.GetApps();
            ddlApp.DataBind();

            if(ddlApp.Items.Cast<ListItem>().Any(p => p.Value == AppId.ToString()))
                ddlApp.SelectedValue = AppId.ToString();
        }

        protected void ChangeTemplate()
        {
            if (UserMayEditThisModule)
            {
                var TemplateID = int.Parse(ddlTemplate.SelectedValue);

                // Return if current TemplateID is already set
                if (Template != null && TemplateID == Template.TemplateID)
                    return;

                new SexyContent(ZoneId, AppId, false).UpdateTemplateForGroup(Sexy.GetContentGroupIDFromModule(ModuleId), TemplateID,
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
        }

        protected void ddlApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppId = int.Parse(ddlApp.SelectedValue);
            Response.Redirect(Request.RawUrl);
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
                        new SexyContent(ZoneId, AppId, false).AddContentGroupItem(Elements.First().GroupId, UserId, Elements.First().TemplateId, null, Elements.Where(el => el.Id == int.Parse(hfContentGroupItemID.Value)).Single().SortOrder + 1, true, ContentGroupItemType.Content, false);
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null));
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
                        if (Elements.Any() && Elements.First().TemplateId.HasValue)
                        {
                            Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), ModuleActionType.EditContent, "edititem", "edit.gif", UrlUtils.PopUpUrl(Sexy.GetElementEditLink(Elements.First().GroupID, Elements.First().SortOrder, ModuleId, TabId, ""), this, PortalSettings, false, false), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                        }
                    }
                    else
                    {
                        // Edit List
                        Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"), ModuleActionType.ContentOptions, "editlist", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditList, false, "mid", this.ModuleId.ToString(), SexyContent.ContentGroupIDString, Elements.First().GroupID.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                    }

                    if (Elements.Any() && Elements.First().TemplateId.HasValue && Template != null && Template.UseForList)
                    {
                        // Add Item (this action will do a postback)
                        Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), SexyContent.ControlKeys.AddItem, SexyContent.ControlKeys.AddItem, "add.gif", "", true, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                    }

                    // Settings Button
                    if (Sexy.GetVisibleTemplates(PortalSettings.PortalId).Any())
                    {
                        Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), ModuleActionType.AddContent, "settings", "action_settings.gif", EditUrl(SexyContent.ControlKeys.SettingsWrapper), false, SecurityAccessLevel.Edit, true, false); }

                    if (!Sexy.SexyContentDesignersGroupConfigured(PortalId) || Sexy.IsInSexyContentDesignersGroup(UserInfo))
                    {
                        if (Template != null)
                        {
                            // Edit Template Button
                            Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", this.ModuleId.ToString(), "TemplateID", Template.TemplateID.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, true);
                        }

                        // Administrator functions
                        Actions.Add(GetNextActionID(), "Admin", "Admin.Action",
                                    "gettingstarted", "action_settings.gif", EditUrl("", "", "gettingstarted", SexyContent.AppIDString + "=" + AppId),
                                    false, SecurityAccessLevel.Admin, true, false);

                        // App Management
                        if(!IsContentApp)
                            Actions.Add(GetNextActionID(), "App Management", "AppManagement.Action",
                                    "appmanagement", "action_settings.gif", EditUrl("", "", "appmanagement"),
                                    false, SecurityAccessLevel.Admin, true, false);
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
                    Sexy.AddContentGroupItem(Elements.First().GroupID, UserId, Template.TemplateID, null, null, true, ContentGroupItemType.Content, false);
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