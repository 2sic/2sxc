using System;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using ToSic.SexyContent.Internal;

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

            if (!EnsureUpgrade(pnlError))
                return;

            base.Page_Load(sender, e);
        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (!SexyContentModuleUpgrade.UpgradeComplete)
                return;

            try
            {
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;

                if (!isSharedModule)
                {
                    // If not fully configured, show stuff
                    if (UserMayEditThisModule)
                        pnlTemplateChooser.Visible = true;

                    if (AppId.HasValue)
                        new DnnStuffToRefactor().EnsurePortalIsConfigured(SxcContext, Server, ControlPath);
                }

                if (AppId.HasValue)
                {
                    if (ContentGroup.Content.Any() && Template != null)
                        ProcessView(phOutput, pnlError, pnlMessage);
                    else if (!IsContentApp && UserMayEditThisModule)
                    // Select first available template automatically if it's not set yet - then refresh page
                    {
                        var templates = SxcContext.AppTemplates.GetAvailableTemplatesForSelector(ModuleConfiguration.ModuleID, SxcContext.AppContentGroups).ToList();
                        if (templates.Any())
                        {
                            SxcContext.AppContentGroups.SetPreviewTemplateId(ModuleConfiguration.ModuleID, templates.First().TemplateId);
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


        protected bool EnsureUpgrade(Panel pnlError)
        {
            // Upgrade success check - show message if upgrade did not run successfully
            if (UserInfo.IsSuperUser && !SexyContentModuleUpgrade.UpgradeComplete)
            {
                if (Request.QueryString["finishUpgrade"] == "true")
                    SexyContentModuleUpgrade.FinishAbortedUpgrade();

                if (SexyContentModuleUpgrade.IsUpgradeRunning)
                    ShowError("It looks like a 2sxc upgrade is currently running. Please wait for the operation to complete (the upgrade may take a few minutes).", pnlError, "", false);
                else
                    ShowError("Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>more</a>). Please click the following button to finish the upgrade:<br><a class='dnnPrimaryAction' href='?finishUpgrade=true'>Finish Upgrade</a>", pnlError, "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.", false);

                return false;
            }

            return true;
        }

    }
}