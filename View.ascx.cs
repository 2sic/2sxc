using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent
{
    public partial class View : PortalModuleBase, IActionable
    {
        #region basic properties like Sexy, App, Zone, etc.
        internal ModuleContentBlock ContentBlock;
        protected SxcInstance _sxcInstance => ContentBlock.SxcInstance;

        protected int? ZoneId => _sxcInstance?.ZoneId ?? 0;

        protected int? AppId => ContentBlock.ContentGroupExists ? _sxcInstance?.AppId : null; // some systems rely on appid being blank to adapt behaviour if nothing is saved yet

        protected Template Template => _sxcInstance.Template;
        #endregion


        protected void Page_Init(object sender, EventArgs e)
        {
            ContentBlock = new ModuleContentBlock(ModuleConfiguration);
        }


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

            #region If logged in, inject Edit JavaScript, and delete / add items
            // register scripts and css
            try
            {
                var renderHelp = new RenderingHelpers(_sxcInstance);
                renderHelp.RegisterClientDependencies(Page, string.IsNullOrEmpty(Request.QueryString["debug"]));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
            #endregion
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
                // check things if it's a module of this portal (ensure everything is ok, etc.)
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;
                if (!isSharedModule && !ContentBlock.ContentGroupExists)
                    new DnnStuffToRefactor().EnsurePortalIsConfigured(_sxcInstance, Server, ControlPath);
                ProcessView();
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void ProcessView()
        {
            string renderedTemplate;
            #region #1 check case data-transfer only vs. really render

            if (Request.QueryString["type"] == "data")
            {
                // note that initializing the engine will also initialize any custom data-changes
                // so this is needed, even if we will only render to JSON afterwards
                _sxcInstance.GetRenderingEngine(InstancePurposes.PublishData);
                renderedTemplate = StreamJsonToClient(_sxcInstance.Data);
            }
            else
                renderedTemplate = _sxcInstance.Render().ToString();

            //var renderedTemplate = GetRenderedTemplateOrJson();
            #endregion

            // If standalone is specified, output just the template without anything else
            if (Request.QueryString["standalone"] == "true")
                SendJsonFeedAndCloseRequest(renderedTemplate);
            else
                phOutput.Controls.Add(new LiteralControl(renderedTemplate));
        }


        #region JSON stuff to transport data in this module as a JSON stream
        private void SendJsonFeedAndCloseRequest(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        private string StreamJsonToClient(ViewDataSource dataSource)
        {
            string renderedTemplate;
            if (dataSource.Publish.Enabled)
            {
                var publishedStreams = dataSource.Publish.Streams;
                renderedTemplate = GetJsonFromStreams(dataSource,
                    publishedStreams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                Response.StatusCode = 403;
                var moduleTitle = new ModuleController().GetModule(ModuleId).ModuleTitle;
                renderedTemplate =
                    JsonConvert.SerializeObject(
                        new
                        {
                            error =
                                "2sxc Content (" + ModuleId + "): " +
                                String.Format(LocalizeString("EnableDataPublishing.Text"), ModuleId, moduleTitle)
                        });
                Response.TrySkipIisCustomErrors = true;
            }
            Response.ContentType = "application/json";
            return renderedTemplate;
        }

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        public string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            var y = streamsToPublish.Where(k => source.Out.ContainsKey(k)).ToDictionary(k => k, s => new
            {
                List = (from c in source.Out[s].List select new DynamicEntity(c.Value, new[] { language }, _sxcInstance).ToDictionary() /*Sexy.ToDictionary(c.Value, language)*/).ToList()
            });

            return JsonConvert.SerializeObject(y);
        }
        #endregion



        #region Security Check
        private SxcInstance _sxcInstanceForSecurityChecks;
        protected bool UserMayEditThisModule
        {
            get
            {
                if (_sxcInstanceForSecurityChecks == null)
                    _sxcInstanceForSecurityChecks = _sxcInstance
                        ?? new ModuleContentBlock(ModuleConfiguration).SxcInstance;
                return _sxcInstanceForSecurityChecks?.Environment?.Permissions.UserMayEditContent ?? false;
            }
        }
        #endregion


        #region ModuleActions on THIS DNN-Module

        /// <summary>
        /// Causes DNN to create the menu with all actions like edit entity, new, etc.
        /// </summary>
        private ModuleActionCollection _ModuleActions;
        public ModuleActionCollection ModuleActions
        {
            get
            {
                try
                {
                    if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
                        _ModuleActions = new ModuleActionCollection();

                    if (_ModuleActions != null) return _ModuleActions;

                    InitModuleActions();
                    return _ModuleActions;
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
            _ModuleActions = new ModuleActionCollection();
            var actions = _ModuleActions;

            if (ZoneId.HasValue && AppId.HasValue)
            {
                // Edit item
                if (!Template?.UseForList ?? false)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
                        SecurityAccessLevel.Edit, true, false);

                // Add Item
                if (Template != null && Template.UseForList)
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
                if (ZoneId.HasValue && AppId.HasValue && Template != null)
                    actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent,
                        "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test",
                        true,
                        SecurityAccessLevel.Edit, true, false);

                // App management
                if (ZoneId.HasValue && AppId.HasValue)
                    actions.Add(GetNextActionID(), "Admin" + (_sxcInstance.IsContentApp ? "" : " " + _sxcInstance.App.Name), "",
                        "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                        SecurityAccessLevel.Admin, true, false);

                // Zone management (app list)
                if (!_sxcInstance.IsContentApp)
                    actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif",
                        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                        SecurityAccessLevel.Admin, true, false);
            }
        }

        #endregion

    }
}