using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.SexyContent.GettingStarted;

namespace ToSic.SexyContent
{
    public partial class View : SexyViewContentOrApp, IActionable
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset messages visible states
            pnlMessage.Visible = false;
            pnlError.Visible = false;

            base.Page_Load(sender, e);
        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
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
                    var templateChooserVisible = !AppId.HasValue || !Elements.Any() || !Elements.First().TemplateId.HasValue || Elements.All(p => !p.EntityId.HasValue);

                    if (Settings.ContainsKey(SexyContent.SettingsShowTemplateChooser))
                        templateChooserVisible = Boolean.Parse(Settings[SexyContent.SettingsShowTemplateChooser].ToString());

                    // If not fully configured, show stuff
                    if (templateChooserVisible)
                        ShowTemplateChooser();

                    if (AppId.HasValue && !Sexy.PortalIsConfigured(Server, ControlPath))
                        Sexy.ConfigurePortal(Server);

                    if (AppId.HasValue && Elements.Any() && Elements.First().TemplateId.HasValue)
                        ProcessView(phOutput, pnlError, pnlMessage);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }


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
                    ddlContentType.Enabled = Elements.Count <= 1 && (!Elements.Any(e => e.EntityId.HasValue));

                    if (Template != null)
                        ddlContentType.SelectedValue = Template.AttributeSetID.ToString();

                    BindTemplateDropDown();
                }

            }

            ddlContentType.Visible = IsContentApp;
            ddlTemplate.Enabled = IsContentApp || AppId.HasValue;
        }

        protected void BindTemplateDropDown()
        {
            IEnumerable<Template> TemplatesToChoose;

            if (Elements.Any(e => e.EntityId.HasValue))
                TemplatesToChoose = Sexy.GetCompatibleTemplates(PortalId, Elements.First().GroupId).Where(p => !p.IsHidden);
            else if (Elements.Count <= 1)
                TemplatesToChoose = Sexy.GetVisibleTemplates(PortalId);
            else
                TemplatesToChoose = Sexy.GetVisibleListTemplates(PortalId);

            // Make only templates from chosen content type shown if content type is set
            if (ddlContentType.SelectedValue != "0")
                TemplatesToChoose = TemplatesToChoose.Where(p => p.AttributeSetID == int.Parse(ddlContentType.SelectedValue));

            ddlTemplate.DataSource = TemplatesToChoose;
            ddlTemplate.DataBind();
            // If the current data is a list of entities, don't allow changing back to no template
            if(Elements.Count <= 1 && (!Elements.Any(e => e.EntityId.HasValue)))
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

        protected void ChangeTemplate()
        {
            if (UserMayEditThisModule && !String.IsNullOrEmpty(ddlTemplate.SelectedValue) && AppId.HasValue)
            {
                var TemplateID = int.Parse(ddlTemplate.SelectedValue);

                // Return if current TemplateID is already set
                if (Template != null && TemplateID == Template.TemplateID)
                    return;

                new SexyContent(ZoneId.Value, AppId.Value, false).UpdateTemplateForGroup(Sexy.GetContentGroupIdFromModule(ModuleId), TemplateID,
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


        #region ModuleActions

        /// <summary>
        /// Causes DNN to create the menu with all actions like edit entity, new, etc.
        /// </summary>
        private ModuleActionCollection _ModuleActions;
        public ModuleActionCollection ModuleActions
        {
            get
            {
                if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                    return new ModuleActionCollection();

                if (_ModuleActions == null)
                {
                    _ModuleActions = new ModuleActionCollection();
                    ModuleActionCollection Actions = _ModuleActions;

                    try
                    {
                        if (ZoneId.HasValue && AppId.HasValue)
                        {
                            if (!IsList)
                            {
                                if (Elements.Any() && Elements.First().TemplateId.HasValue)
                                {
                                    var editLink = Sexy.GetElementEditLink(Elements.First().GroupID, Elements.First().SortOrder, ModuleId, TabId, "");
                                    Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"),
                                        ModuleActionType.EditContent, "edititem", "edit.gif", PortalSettings.EnablePopUps ?
                                        UrlUtils.PopUpUrl(editLink, this, PortalSettings, false, false) : editLink, false,
                                        SecurityAccessLevel.Edit, true, false);
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
                                    SecurityAccessLevel.Edit, true, false);
                            }

                            if (Elements.Any() && Elements.First().TemplateId.HasValue && Template != null &&
                                Template.UseForList)
                            {
                                // Add Item
                                Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxc(" + this.ModuleId + ").manage.action({ 'action':'add', 'useModuleList': true });", "test", true,
                                    SecurityAccessLevel.Edit, true, false);
                            }

                            // Change layout button
                            if (Sexy.GetVisibleTemplates(PortalSettings.PortalId).Any() && Template != null)
                            {
                                Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"),
                                    "", "", "action_settings.gif", "javascript:$2sxc(" + this.ModuleId + ").manage._setTemplateChooserState(true);", "test", true,
                                    SecurityAccessLevel.Edit, true, false);
                            }
                        }

                        if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId) || SexyContent.IsInSexyContentDesignersGroup(UserInfo))
                        {
                            if (ZoneId.HasValue && AppId.HasValue && Template != null)
                            {
                                // Edit Template Button
                                Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", this.ModuleId.ToString(), "TemplateID", Template.TemplateID.ToString()), false, SecurityAccessLevel.Admin, true, true);
                            }

                            // Administrator functions
                            if (ZoneId.HasValue && AppId.HasValue)
                                Actions.Add(GetNextActionID(), "Admin", "Admin.Action",
                                            "gettingstarted", "action_settings.gif", EditUrl("", "", "gettingstarted", SexyContent.AppIDString + "=" + AppId),
                                            false, SecurityAccessLevel.Admin, true, false);

                            // App Management
                            if (!IsContentApp)
                                Actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action",
                                        "appmanagement", "action_settings.gif", EditUrl("", "", "appmanagement"),
                                        false, SecurityAccessLevel.Admin, true, false);
                        }
                    }
                    catch (Exception e)
                    {
                        Exceptions.ProcessModuleLoadException(this, e);
                    }
                }
                return _ModuleActions;
            }
        }

        #endregion

    }
}