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
                
                var noTemplatesYet = !Sexy.GetVisibleTemplates(PortalId).Any();

                // If there are no templates configured - show "getting started" frame
                if (noTemplatesYet && IsEditable && UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    pnlGetStarted.Visible = true;
                    var gettingStartedControl = (GettingStartedFrame)LoadControl("~/DesktopModules/ToSIC_SexyContent/SexyContent/GettingStarted/GettingStartedFrame.ascx");
                    gettingStartedControl.ModuleID = this.ModuleId;
                    gettingStartedControl.ModuleConfiguration = this.ModuleConfiguration;
                    pnlGetStarted.Controls.Add(gettingStartedControl);
                }

                if (UserMayEditThisModule)
                    pnlTemplateChooser.Visible = true;

                if (AppId.HasValue && !Sexy.PortalIsConfigured(Server, ControlPath))
                    Sexy.ConfigurePortal(Server);

                if (AppId.HasValue && Elements.Any() && Elements.First().TemplateId.HasValue)
                    ProcessView(phOutput, pnlError, pnlMessage);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
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
                                    //var editLink = Sexy.GetElementEditLink(Elements.First().GroupID, Elements.First().SortOrder, ModuleId, TabId, "");
                                    //Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"),
                                    //    ModuleActionType.EditContent, "edititem", "edit.gif", PortalSettings.EnablePopUps ?
                                    //    UrlUtils.PopUpUrl(editLink, this, PortalSettings, false, false) : editLink, false,
                                    //    SecurityAccessLevel.Edit, true, false);

                                    // Add Item
                                    Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").edit();", "test", true,
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
                                Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true, false);
                            }

                            // Change layout button
                            Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").changeLayoutOrContent();", false, SecurityAccessLevel.Edit, true, false);
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