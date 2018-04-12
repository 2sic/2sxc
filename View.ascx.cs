using System;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Dnn;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        private SxcInstance _sxci;

        protected SxcInstance SxcInstance => _sxci ?? (_sxci = new ModuleContentBlock(
                                              new DnnInstanceInfo(ModuleConfiguration),
                                              Log,
                                              new DnnTenant(new PortalSettings(ModuleConfiguration.OwnerPortalID)))
                                          .SxcInstance);

        private Log Log { get; } = new Log("Sxc.View");

        /// <summary>
        /// Page Load event - preload template chooser if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // always do this, part of the guarantee that everything will work
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }

        /// <summary>s
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                // check things if it's a module of this portal (ensure everything is ok, etc.)
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;
                if (!isSharedModule && !SxcInstance.ContentBlock.ContentGroupExists && SxcInstance.App != null)
                    new DnnStuffToRefactor().EnsureTenantIsConfigured(SxcInstance, Server, ControlPath);

                var renderNaked = Request.QueryString["standalone"] == "true";
                if (renderNaked)
                    SxcInstance.RenderWithDiv = false;
                var renderedTemplate = SxcInstance.Render().ToString();

                // call this after rendering templates, because the template may change what resources are registered
                RegisterResources();

                // optional detailed logging
                try
                {
                    if (Request.QueryString["debug"] == "true")
                    {
                        // only attach debug, if it has been enabled for the current time period
                        if (UserInfo.IsSuperUser || Logging.EnableLogging(GlobalConfiguration.Configuration
                            .Properties))
                            renderedTemplate += HtmlLog();
                    }
                }
                catch { /* ignore */}

                // If standalone is specified, output just the template without anything else
                if (renderNaked)
                    SendStandalone(renderedTemplate);
                else
                    phOutput.Controls.Add(new LiteralControl(renderedTemplate));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }


        private void SendStandalone(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private string HtmlLog() => Log.Dump(" - ", "<!-- 2sxc extensive log for " + ModuleId + "\n", "-->");


        #region Security Check

        //protected bool UserMayEditThisModule => SxcInstance?.UserMayEdit ?? false;
        #endregion


        /// <summary>
        /// Register all client parts (css/js, anti-forgery-token)
        /// to enable editing or data work
        /// </summary>
        private void RegisterResources()
        {
            var loadJs = SxcInstance?.UiAddEditApi ?? false;
            var loadCss = SxcInstance?.UiAddEditUi ?? false;

            if (!loadJs && !loadCss) return;

            Log.Add("user is editor, or template requested js/css, will add client material");

            #region If logged in, inject Edit JavaScript, and delete / add items

            // register scripts and css
            try
            {
                new DnnRenderingHelpers(SxcInstance, Log).RegisterClientDependencies(Page, loadJs, loadCss);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

            #endregion
        }



        #region ModuleActions on THIS DNN-Module

        /// <summary>
        /// Causes DNN to create the menu with all actions like edit entity, new, etc.
        /// </summary>
        private ModuleActionCollection _moduleActions;
        public ModuleActionCollection ModuleActions
        {
            get
            {
                try
                {
                    if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                        _moduleActions = new ModuleActionCollection();

                    if (_moduleActions != null) return _moduleActions;

                    InitModuleActions();
                    return _moduleActions;
                }
                catch (Exception e)
                {
                    Exceptions.LogException(e);
                    return new ModuleActionCollection();
                }
            }
        }

        private void InitModuleActions()
        {
            _moduleActions = new ModuleActionCollection();
            var actions = _moduleActions;
            var appIsKnown = SxcInstance.AppId > 0;

            if (appIsKnown)
            {
                // Edit item
                if (!SxcInstance.Template?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
                        SecurityAccessLevel.Edit, true, false);

                // Add Item
                if (SxcInstance.Template?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true,
                        false);

                // Change layout button
                actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif",
                    "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false,
                    SecurityAccessLevel.Edit, true, false);
            }

            if (!SecurityHelpers.SexyContentDesignersGroupConfigured(PortalId) ||
                SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo))
            {
                // Edit Template Button
                if (appIsKnown && SxcInstance.Template != null)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                        "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test",
                        true,
                        SecurityAccessLevel.Edit, true, false);

                // App management
                if (appIsKnown)
                    actions.Add(GetNextActionID(), "Admin" + (SxcInstance.IsContentApp ? "" : " " + SxcInstance.App?.Name), "",
                        "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                        SecurityAccessLevel.Admin, true, false);

                // Zone management (app list)
                if (!SxcInstance.IsContentApp)
                    actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                        SecurityAccessLevel.Admin, true, false);
            }
        }

        #endregion

    }
}