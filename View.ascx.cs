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
        protected new void Page_Load(object sender, EventArgs e)
        {
            pnlError.Visible = false;

            var notReady = Installer.CheckUpgradeMessage(UserInfo.IsSuperUser);
            if(!string.IsNullOrEmpty(notReady))
                ShowError(notReady, pnlError, notReady, false);

            base.Page_Load(sender, e);
        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {

            if (!Installer.UpgradeComplete)
                return;

            try
            {
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;

                // check things if it's a module of this portal (ensure everything is ok, etc.)
                if (!isSharedModule)
                {
                    // if editable, include template chooser
                    if (UserMayEditThisModule)
                        pnlTemplateChooser.Visible = true;

                    if (SettingsAreStored)// AppId.HasValue)
                        new DnnStuffToRefactor().EnsurePortalIsConfigured(_sxcInstance, Server, ControlPath);
                }

                if (SettingsAreStored) // AppId.HasValue)
                {
                    if (ContentGroup.Content.Any() && Template != null)
                        ProcessView(phOutput, pnlError);//, pnlMessage);
                    else if (!IsContentApp && UserMayEditThisModule)
                    // Select first available template automatically if it's not set yet - then refresh page
                    {
                        var templates = _sxcInstance.AppTemplates.GetAvailableTemplatesForSelector(ModuleConfiguration.ModuleID, _sxcInstance.AppContentGroups).ToList();
                        if (templates.Any())
                        {
                            _sxcInstance.AppContentGroups.SetPreviewTemplateId(ModuleConfiguration.ModuleID, templates.First().TemplateId);
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