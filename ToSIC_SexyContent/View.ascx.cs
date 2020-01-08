using System;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        private CmsBlock _cmsBlock;
        private bool _cmsBlockLoaded;

        protected CmsBlock CmsBlock
        {
            get
            {
                if (_cmsBlockLoaded) return _cmsBlock;
                _cmsBlockLoaded = true;
                _cmsBlock = new BlockFromModule(
                        new DnnContainer(ModuleConfiguration),
                        Log,
                        new DnnTenant(new PortalSettings(ModuleConfiguration.OwnerPortalID)))
                    .CmsInstance as CmsBlock;
                return _cmsBlock;
            }
        }

        private ILog Log { get; } = new Log("Sxc.View");

        /// <summary>
        /// Page Load event
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            // add to insights-history for analytics
            History.Add("module", Log);

            // always do this, part of the guarantee that everything will work
            // 2020-01-06 2sxc 10.25 - moved away to DnnRenderingHelpers
            // to only load when we're actually activating the JS.
            // might be a breaking change for some code that "just worked" before
            //ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            
            // new mechanism in 10.25
            // this must happen in Page-Load, so we know what supporting scripts to add
            // at this stage of the lifecycle
            Html = RenderViewAndGatherJsCssSpecs();

            // call this after rendering templates, because the template may change what resources are registered
            TryCatchAndLogToDnn(() => new DnnClientResources(Page, CmsBlock, Log).AddEverything());
        }

        public bool RenderNaked 
            => _renderNaked ?? (_renderNaked = Request.QueryString["standalone"] == "true").Value;
        private bool? _renderNaked;

        protected string Html;

        /// <summary>s
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            TryCatchAndLogToDnn(() =>
            {
                // If standalone is specified, output just the template without anything else
                if (RenderNaked)
                    SendStandalone(Html);
                else
                    phOutput.Controls.Add(new LiteralControl(Html));
            });
        }

        private string RenderViewAndGatherJsCssSpecs()
        {
            var renderedTemplate = "";
            var timerWrap = Log.Call(message: $"module {ModuleId} on page {TabId}", useTimer: true);
            try
            {
                // throw better error if SxcInstance isn't available
                // not sure if this doesn't have side-effects...
                if (CmsBlock == null)
                    throw new Exception("Error - can't find 2sxc instance configuration. " +
                                        "Probably trying to show an app or content that has been deleted.");

                // check things if it's a module of this portal (ensure everything is ok, etc.)
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;
                if (!isSharedModule && !CmsBlock.Block.ContentGroupExists && CmsBlock.App != null)
                    new DnnTenantSettings().EnsureTenantIsConfigured(CmsBlock, Server, ControlPath);

                //renderNaked = Request.QueryString["standalone"] == "true";
                if (RenderNaked)
                    CmsBlock.RenderWithDiv = false;
                renderedTemplate = CmsBlock.Render().ToString();

                // optional detailed logging
                try
                {
                    // if in debug mode and is super-user (or it's been enabled for all), then add to page debug
                    if (Request.QueryString["debug"] == "true")
                        if (UserInfo.IsSuperUser
                            || DnnLogging.EnableLogging(GlobalConfiguration.Configuration.Properties))
                            renderedTemplate += HtmlLog();
                }
                catch
                {
                    /* ignore */
                }
            }
            catch (Exception ex)
            {
                timerWrap(null);
                Exceptions.ProcessModuleLoadException(this, ex);
            }
            finally
            {
                timerWrap(null);
            }

            return renderedTemplate;
        }


        private void SendStandalone(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private string HtmlLog() 
            => Log.Dump(" - ", "<!-- 2sxc insights for " + ModuleId + "\n", "-->");


        private void TryCatchAndLogToDnn(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
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
            var appIsKnown = CmsBlock.Block.AppId > 0;

            if (appIsKnown)
            {
                // Edit item
                if (!CmsBlock.View?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
                        SecurityAccessLevel.Edit, true, false);

                // Add Item
                if (CmsBlock.View?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true,
                        false);

                // Change layout button
                actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif",
                    "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false,
                    SecurityAccessLevel.Edit, true, false);
            }

            if (!DnnSecurity.SexyContentDesignersGroupConfigured(PortalId) ||
                DnnSecurity.IsInSexyContentDesignersGroup(UserInfo))
            {
                // Edit Template Button
                if (appIsKnown && CmsBlock.View != null)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                        "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test",
                        true,
                        SecurityAccessLevel.Edit, true, false);

                // App management
                if (appIsKnown)
                    actions.Add(GetNextActionID(), "Admin" + (CmsBlock.Block.IsContentApp ? "" : " " + CmsBlock.App?.Name), "",
                        "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                        SecurityAccessLevel.Admin, true, false);

                // Zone management (app list)
                if (!CmsBlock.Block.IsContentApp)
                    actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                        SecurityAccessLevel.Admin, true, false);
            }
        }

        #endregion

    }
}