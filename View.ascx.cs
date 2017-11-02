using System;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        private SxcInstance _sxci;
        protected SxcInstance SxcI => _sxci ?? (_sxci = new ModuleContentBlock(ModuleConfiguration, Log).SxcInstance);

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

            if (!UserMayEditThisModule) return;

            Log.Add("user is editor, will add context");
            #region If logged in, inject Edit JavaScript, and delete / add items
            // register scripts and css
            try
            {
                //var renderHelp = new RenderingHelpers(SxcI);
                new RenderingHelpers(SxcI, Log).RegisterClientDependencies(Page);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
            #endregion
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
                if (!isSharedModule && !SxcI.ContentBlock.ContentGroupExists && SxcI.App != null)
                    new DnnStuffToRefactor().EnsurePortalIsConfigured(SxcI, Server, ControlPath);

                var renderNaked = Request.QueryString["standalone"] == "true";
                if (renderNaked)
                    SxcI.RenderWithDiv = false;
                var renderedTemplate = SxcI.Render().ToString();

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

        private string HtmlLog()
        {
            return Log.Dump(" - ", "<!-- 2sxc extensive log for " + ModuleId + "\n", "-->");
            //var lg = new StringBuilder("<!-- 2sxc extensive log for " + ModuleId + "\n");
            //Log.Entries.ForEach(e => lg.AppendLine(e.Source + " - " + e.Message));
            //lg.Append("-->");
            //return lg.ToString();
        }


        #region Security Check
        protected bool UserMayEditThisModule => SxcI?.Environment?.Permissions.UserMayEditContent ?? false;
        #endregion


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
            var appIsKnown = (SxcI.AppId.HasValue);

            if (appIsKnown)
            {
                // Edit item
                if (!SxcI.Template?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
                        SecurityAccessLevel.Edit, true, false);

                // Add Item
                if (SxcI.Template?.UseForList ?? false)
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
                if (appIsKnown && SxcI.Template != null)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                        "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test",
                        true,
                        SecurityAccessLevel.Edit, true, false);

                // App management
                if (appIsKnown)
                    actions.Add(GetNextActionID(), "Admin" + (SxcI.IsContentApp ? "" : " " + SxcI.App.Name), "",
                        "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                        SecurityAccessLevel.Admin, true, false);

                // Zone management (app list)
                if (!SxcI.IsContentApp)
                    actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                        SecurityAccessLevel.Admin, true, false);
            }
        }
        #endregion

    }
}