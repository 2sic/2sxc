using System;
using System.Linq;
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

    }
}