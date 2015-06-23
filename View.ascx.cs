using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using ToSic.SexyContent.GettingStarted;

// Warning: This code is 99% identical View.ascx.cs and ViewApp.ascx.cs. These should be merged! Dangerous to keep them apart

namespace ToSic.SexyContent
{
    public partial class View : SexyViewContentOrApp, IActionable
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
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;

                if (!isSharedModule)
                {
                    var noTemplatesYet = !Sexy.Templates.GetVisibleTemplates().Any();

                    // If there are no templates configured - show "getting started" frame
                    if (noTemplatesYet && IsEditable && UserInfo.IsInRole(PortalSettings.AdministratorRoleName))
                    {
                        pnlGetStarted.Visible = true;
                        var gettingStartedControl = (GettingStartedFrame)LoadControl("~/DesktopModules/ToSIC_SexyContent/SexyContent/GettingStarted/GettingStartedFrame.ascx");
                        gettingStartedControl.ModuleID = ModuleId;
                        gettingStartedControl.ModuleConfiguration = ModuleConfiguration;
                        pnlGetStarted.Controls.Add(gettingStartedControl);
                    }

                    // If not fully configured, show stuff
                    if (UserMayEditThisModule)
                        pnlTemplateChooser.Visible = true;

                    if (AppId.HasValue && !Sexy.PortalIsConfigured(Server, ControlPath))
                        Sexy.ConfigurePortal(Server);
                }

                if (AppId.HasValue)
                {
                    if (ContentGroup.Content.Any() && Template != null)
                        ProcessView(phOutput, pnlError, pnlMessage);
                    else if(!IsContentApp && UserMayEditThisModule) // Select first available template automatically if it's not set yet - then refresh page
                    {
                        var templates = Sexy.GetAvailableTemplatesForSelector(ModuleConfiguration).ToList();
                        if (templates.Any())
                        {
                            Sexy.ContentGroups.SetPreviewTemplateId(ModuleConfiguration.ModuleID, templates.First().TemplateId);
                            Response.Redirect(Request.RawUrl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

    }
}