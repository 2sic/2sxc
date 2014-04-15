using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using System.Collections.Generic;
using DotNetNuke.Security;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System.Linq;
using DotNetNuke.Entities.Modules;
using ToSic.Eav;
using DotNetNuke.Entities.Modules.Actions;

namespace ToSic.SexyContent
{
    public partial class ViewApp : SexyControlEditBase, IActionable
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
                            //attributeSetName = Template != null ? Sexy.ContentContext.GetAttributeSet(Template.AttributeSetID).StaticName : null,
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

                if (!SexyContent.PortalIsConfigured(Server, ControlPath))
                    pnlMissingConfiguration.Visible = true;

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
            if (!Page.IsPostBack && UserMayEditThisModule)
            {
                pnlTemplateChooser.Visible = true;
                ddlTemplate.Visible = AppId.HasValue;

                if (AppId.HasValue)
                {
                    BindTemplateDropDown();
                }

                if (!IsContentApp)
                {
                    ddlApp.Visible = true;
                    BindAppDropDown();
                }

            }
        }

        protected void BindTemplateDropDown()
        {
            IEnumerable<Template> TemplatesToChoose;

            if (!IsList)
                TemplatesToChoose = Sexy.GetVisibleTemplates(PortalId);
            else
                TemplatesToChoose = Sexy.GetVisibleListTemplates(PortalId);

            ddlTemplate.DataSource = TemplatesToChoose;
            ddlTemplate.DataBind();
            // If the current data is a list of entities, don't allow changing back to no template
            if(!IsList)
                ddlTemplate.Items.Insert(0, new ListItem(LocalizeString("ddlTemplateDefaultItem.Text"), "0"));

            // If there are elements and the selected template exists in the list, select that
            if (Elements.Any() && ddlTemplate.Items.FindByValue(Elements.First().TemplateId.ToString()) != null)
                ddlTemplate.SelectedValue = Elements.First().TemplateId.ToString();
            else if(ddlTemplate.Items.Count > 1)
            {
                ddlTemplate.SelectedIndex = 1;
                ChangeTemplate();
            }

            if (ddlTemplate.Items.Count == 2)
            {
                ddlTemplate.Visible = false;
            }

        }

        private void BindAppDropDown()
        {
            ddlApp.DataSource = SexyContent.GetApps(ZoneId.Value, false).Where(a => !a.Hidden);
            ddlApp.DataBind();

            if(ddlApp.Items.Cast<ListItem>().Any(p => p.Value == AppId.ToString()))
                ddlApp.SelectedValue = AppId.ToString();
        }

        protected void ChangeTemplate()
        {
            if (UserMayEditThisModule && !String.IsNullOrEmpty(ddlTemplate.SelectedValue))
            {
                var TemplateID = int.Parse(ddlTemplate.SelectedValue);

                // Return if current TemplateID is already set
                if (Template != null && TemplateID == Template.TemplateID)
                    return;

                new SexyContent(ZoneId.Value, AppId.Value, false).UpdateTemplateForGroup(Sexy.GetContentGroupIDFromModule(ModuleId), TemplateID,
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

        protected void ddlApp_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppId = int.Parse(ddlApp.SelectedValue);
            ddlTemplate.SelectedValue = "0";
            ChangeTemplate();
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
                            Actions.Add(GetNextActionID(), "Admin " + Sexy.App.Name, "Admin.Action",
                                        "gettingstarted", "action_settings.gif", EditUrl("", "", "gettingstarted", SexyContent.AppIDString + "=" + AppId),
                                        false, SecurityAccessLevel.Admin, true, false);

                        // App Management
                        if(!IsContentApp)
                            Actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action",
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
                    SexyUncached.AddContentGroupItem(Elements.First().GroupID, UserId, Template.TemplateID, null, null, true, ContentGroupItemType.Content, false);
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
            if (!SexyContent.PortalIsConfigured(Server, ControlPath))
            {
                Sexy.ConfigurePortal(Server);
                Response.Redirect(Request.RawUrl);
            }
        }
    }
}