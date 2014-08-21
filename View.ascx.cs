using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using System.Collections.Generic;
using DotNetNuke.Security;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using DotNetNuke.Entities.Modules.Actions;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.GettingStarted;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    public partial class View : SexyControlEditBase, IActionable
    {

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
            if(UserMayEditThisModule && Sexy != null)
            {
                ClientScriptManager ClientScript = Page.ClientScript;
                ClientScript.RegisterClientScriptInclude("ViewEdit", ResolveClientUrl("~/DesktopModules/ToSIC_SexyContent/Js/ViewEdit.js"));

                hfContentGroupItemAction.Visible = true;
                hfContentGroupItemSortOrder.Visible = true;

                ((DotNetNuke.UI.Modules.ModuleHost)this.Parent).Attributes.Add("data-2sxc", (new
                {
                    moduleId = ModuleId,
                    manage = new
                    {
                        isEditMode = UserMayEditThisModule,
                        config = new
                        {
                            portalId = PortalId,
                            tabId = TabId,
                            moduleId = ModuleId,
                            contentGroupId = Elements.Any() ? Elements.First().GroupId : -1,
                            dialogUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId),
                            returnUrl = Request.RawUrl,
                            appPath = AppId.HasValue ? Sexy.App.Path : null,
                            cultureDimension = Sexy.GetCurrentLanguageID(),
                            isList = Template != null && Template.UseForList
                        }
                    }
                }).ToJson());

                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.js", 90);
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.manage.js", 91);
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
                    "mid", this.ModuleId.ToString());
            }
            else
            {
                // If not fully configured, show stuff
                if (!AppId.HasValue || !Elements.Any() || !Elements.First().TemplateId.HasValue || Elements.All(p => !p.EntityId.HasValue))
                    ShowTemplateChooser();

                if (AppId.HasValue && !Sexy.PortalIsConfigured(Server, ControlPath))
                    Sexy.ConfigurePortal(Server);

                if (AppId.HasValue && Elements.Any() && Elements.First().TemplateId.HasValue)
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

            if (DataSource.GetCache(ZoneId.Value, AppId.Value).GetContentType(Template.AttributeSetID) == null)
            {
                ShowError("The contents of this module cannot be displayed because it's located in another VDB.");
                return;
            }

            if (Elements.All(e => e.Content == null))
            {
                var toolbar = IsEditable ? "<ul class='sc-menu' data-toolbar='" + new { sortOrder = Elements.First().SortOrder, useModuleList = true, action = "edit" }.ToJson() + "'></ul>" : "";
                ShowMessage(LocalizeString("NoDemoItem.Text") + " " + toolbar);
                return;
            }

            #endregion

            try
            {
                
                string renderedTemplate = "";
                var engine = EngineFactory.CreateEngine(Template);
                var dataSource = (ViewDataSource)Sexy.GetViewDataSource(this.ModuleId, SexyContent.HasEditPermission(this.ModuleConfiguration), DotNetNuke.Common.Globals.IsEditMode());
                engine.Init(Template, Sexy.App, this.ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView);
                engine.CustomizeData();

                // Output JSON data if type=data in URL
                if (Request.QueryString["type"] == "data")
                {
                    if (dataSource.Publish.Enabled)
                    {
                        var publishedStreams = dataSource.Publish.Streams;
                        renderedTemplate = Sexy.GetJsonFromStreams(dataSource, publishedStreams.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        Response.StatusCode = 403;
                        var moduleTitle = new ModuleController().GetModule(ModuleId).ModuleTitle;
                        renderedTemplate = (new { error = "2sxc Content (" + ModuleId + "): " + String.Format(LocalizeString("EnableDataPublishing.Text"), ModuleId, moduleTitle) }).ToJson();
                        Response.TrySkipIisCustomErrors = true;
                    }
                    Response.ContentType = "application/json";
                }
                else
                {
                    renderedTemplate = engine.Render();
                }

                // If standalone is specified, output just the template without anything else
                if (StandAlone)
                {
                    Response.Clear();
                    Response.Write(renderedTemplate);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                    phOutput.Controls.Add(new LiteralControl(renderedTemplate));
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
            var noTemplatesYet = !AppId.HasValue || !Sexy.GetVisibleTemplates(PortalId).Any();

            // If there are no templates configured
            if (noTemplatesYet && IsEditable && UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
            {
                pnlGetStarted.Visible = true;
                var gettingStartedControl = (GettingStartedFrame)LoadControl("~/DesktopModules/ToSIC_SexyContent/SexyContent/GettingStarted/GettingStartedFrame.ascx");
                gettingStartedControl.ModuleID = this.ModuleId;
                gettingStartedControl.ModuleConfiguration = this.ModuleConfiguration;
                pnlGetStarted.Controls.Add(gettingStartedControl);
            }

            if (!Page.IsPostBack && UserMayEditThisModule)
            {
                pnlTemplateChooser.Visible = true;

                if (AppId.HasValue)
                {
                    ddlContentType.DataSource = Sexy.GetAvailableAttributeSetsForVisibleTemplates(PortalId);
                    ddlContentType.DataBind();
                    ddlContentType.Enabled = Elements.Count <= 1;

                    if (Template != null)
                        ddlContentType.SelectedValue = Template.AttributeSetID.ToString();

                    BindTemplateDropDown();
                }

                if (!IsContentApp)
                {
                    ddlApp.Visible = true;
                    BindAppDropDown();
                }
                    
            }

            ddlContentType.Visible = IsContentApp;
            ddlTemplate.Enabled = IsContentApp || AppId.HasValue;
        }

        protected void BindTemplateDropDown()
        {
            IEnumerable<Template> TemplatesToChoose;

            if (Elements.Count <= 1)
                TemplatesToChoose = Sexy.GetVisibleTemplates(PortalId);
            else
                TemplatesToChoose = Sexy.GetVisibleListTemplates(PortalId);

            // Make only templates from chosen content type shown if content type is set
            if (ddlContentType.SelectedValue != "0")
                TemplatesToChoose = TemplatesToChoose.Where(p => p.AttributeSetID == int.Parse(ddlContentType.SelectedValue));

            ddlTemplate.DataSource = TemplatesToChoose;
            ddlTemplate.DataBind();
            // If the current data is a list of entities, don't allow changing back to no template
            if(Elements.Count <= 1)
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
            ddlApp.DataSource = SexyContent.GetApps(ZoneId.Value, false, Sexy.OwnerPS).Where(a => !a.Hidden);
            ddlApp.DataBind();

            if (ddlApp.Items.Cast<ListItem>().Any(p => p.Value == AppId.ToString()))
            {
                ddlApp.SelectedValue = AppId.ToString();
                ddlApp.Items.Cast<ListItem>().First().Enabled = false;
            }
        }

        protected void ChangeTemplate()
        {
            if (UserMayEditThisModule)
            {
                var TemplateID = int.Parse(ddlTemplate.SelectedValue);

                // Return if current TemplateID is already set
                if (Template != null && TemplateID == Template.TemplateID)
                    return;

                SexyUncached.UpdateTemplateForGroup(Sexy.GetContentGroupIdFromModule(ModuleId), TemplateID,
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
                        new SexyContent(ZoneId.Value, AppId.Value, false).AddContentGroupItem(Elements.First().GroupId, UserId, Elements.First().TemplateId, null, int.Parse(hfContentGroupItemSortOrder.Value) + 1, true, ContentGroupItemType.Content, false);
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(this.TabId, "", null));
                        break;
                }  

                hfContentGroupItemAction.Value = "";
                hfContentGroupItemSortOrder.Value = "";
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
                if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                    return new ModuleActionCollection();

                if (_ModuleActions == null)
                {
                    _ModuleActions = new DotNetNuke.Entities.Modules.Actions.ModuleActionCollection();
                    DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Actions = _ModuleActions;

                    if (ZoneId.HasValue && AppId.HasValue)
                    {
                        if (!IsList)
                        {
                            if (Elements.Any() && Elements.First().TemplateId.HasValue)
                            {
                                Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"),
                                    ModuleActionType.EditContent, "edititem", "edit.gif",
                                    UrlUtils.PopUpUrl(
                                        Sexy.GetElementEditLink(Elements.First().GroupID, Elements.First().SortOrder,
                                            ModuleId, TabId, ""), this, PortalSettings, false, false), false,
                                    DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                            }
                        }
                        else
                        {
                            // Edit List
                            Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"),
                                ModuleActionType.ContentOptions, "editlist", "edit.gif",
                                EditUrl(this.TabId, SexyContent.ControlKeys.EditList, false, "mid",
                                    this.ModuleId.ToString(), SexyContent.ContentGroupIDString,
                                    Elements.First().GroupID.ToString()), false,
                                DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                        }

                        if (Elements.Any() && Elements.First().TemplateId.HasValue && Template != null &&
                            Template.UseForList)
                        {
                            // Add Item (this action will do a postback)
                            Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"),
                                SexyContent.ControlKeys.AddItem, SexyContent.ControlKeys.AddItem, "add.gif", "", true,
                                DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                        }

                        // Settings Button
                        if (Sexy.GetVisibleTemplates(PortalSettings.PortalId).Any() && Template != null)
                        {
                            Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"),
                                ModuleActionType.AddContent, "settings", "action_settings.gif",
                                EditUrl(SexyContent.ControlKeys.SettingsWrapper), false, SecurityAccessLevel.Edit, true,
                                false);
                        }
                    }

                    if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId) || SexyContent.IsInSexyContentDesignersGroup(UserInfo))
                    {
                        if (ZoneId.HasValue && AppId.HasValue && Template != null)
                        {
                            // Edit Template Button
                            Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", this.ModuleId.ToString(), "TemplateID", Template.TemplateID.ToString()), false, DotNetNuke.Security.SecurityAccessLevel.Admin, true, true);
                        }

                        // Administrator functions
                        if(ZoneId.HasValue && AppId.HasValue)
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

        protected void ModuleActions_Click(object sender, ActionEventArgs e)
        {
            switch (e.Action.CommandName)
            {
                case SexyContent.ControlKeys.AddItem:
                    SexyUncached.AddContentGroupItem(Elements.First().GroupID, UserId, Template.TemplateID, null, null, true, ContentGroupItemType.Content, false);
                    Response.Redirect(Request.RawUrl);
                    break;
            }
        }
        #endregion

    }
}