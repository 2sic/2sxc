using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using Newtonsoft.Json;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using IDataSource = ToSic.Eav.DataSources.IDataSource;

namespace ToSic.SexyContent
{
	/// <summary>
	/// This class contains code that are both used in ViewApp.ascx.cs and View.ascx.cs
	/// </summary>
	public class SexyViewContentOrApp : PortalModuleBase
    {
         #region basic properties like Sexy, App, Zone, etc.
        internal ModuleContentBlock ContentBlock;
        protected SxcInstance _sxcInstance => ContentBlock.SxcInstance;

        protected int? ZoneId => _sxcInstance?.ZoneId ?? 0;

        protected int? AppId => ContentBlock.ContentGroupExists ? _sxcInstance?.AppId : null; // some systems rely on appid being blank to adapt behaviour if nothing is saved yet

        protected bool SettingsAreStored => ContentBlock.ContentGroupExists;
        protected Template Template => _sxcInstance.Template;
        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            ContentBlock = new ModuleContentBlock(ModuleConfiguration);
        }

        protected void Page_Load(object sender, EventArgs e)
		{
            // always do this, part of the guarantee that everything will work
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            var renderHelp = new RenderingHelpers(_sxcInstance);


			// If logged in, inject Edit JavaScript, and delete / add items
            if (!UserMayEditThisModule) return;
            try
            {
                // register scripts and css
                renderHelp.RegisterClientDependencies(Page, string.IsNullOrEmpty(Request.QueryString["debug"]));

                // add instance specific infos to the html-tag
                var clientInfos = renderHelp.InfosForTheClientScripts();
                ((ModuleHost) Parent).Attributes.Add("data-2sxc", JsonConvert.SerializeObject(clientInfos));

                // Add some required variables to module host div which are general global 2sxc-infos
                //var globalInfos = renderHelp.RegisterGlobalsAttribute();
                //((ModuleHost)Parent).Attributes.Add("data-2sxc-globals", JsonConvert.SerializeObject(globalInfos));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
		}


        private SxcInstance _sxcInstanceForSecurityChecks;
        protected bool UserMayEditThisModule 
        {
            get
            {
                if (_sxcInstanceForSecurityChecks == null)
                    _sxcInstanceForSecurityChecks = _sxcInstance 
                        ?? new ModuleContentBlock(ModuleConfiguration).SxcInstance ;
                return _sxcInstanceForSecurityChecks?.Environment?.Permissions.UserMayEditContent ?? false;
            }
        }

        private string GetRenderedTemplateOrJson()
	    {
            // note that initializing the engine will also initialize any custom data-changes
            // so this is needed, even if we will only render to JSON afterwards
            if (Request.QueryString["type"] == "data")
            {
                _sxcInstance.GetRenderingEngine(InstancePurposes.PublishData);
                return StreamJsonToClient(_sxcInstance.Data);
            }
            return _sxcInstance.Render().ToString();

	    }


        #region JSON stuff to transport data in this module as a JSON stream
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


        #region DNN-View-Specific stuff which will stay in this class

        /// <summary>
        /// Get the content data and render it with the given template to the page.
        /// </summary>
        protected void ProcessView(PlaceHolder phOutput, Panel pnlError)//, Panel pnlMessage)
        {
            try
            {
                var renderedTemplate = GetRenderedTemplateOrJson();

                // If standalone is specified, output just the template without anything else
                if (Request.QueryString["standalone"] == "true")
                {
                    Response.Clear();
                    Response.Write(renderedTemplate);
                    Response.Flush();
                    Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                    phOutput.Controls.Add(new LiteralControl(renderedTemplate));
            }
            catch (RenderingException rex)
            {
                if (rex.RenderStatus == RenderStatusType.MissingData)
                    ShowMessage(EngineBase.ToolbarForEmptyTemplate, phOutput);// pnlMessage);
                else
                    ShowError(rex.Message, pnlError);

                if (rex.ShouldLog)
                    Exceptions.LogException(rex);
            }
            // Catch all other errors; log them
            catch (Exception Ex)
            {
                ShowError(LocalizeString("TemplateError.Text") + ": " + HttpUtility.HtmlEncode(Ex.ToString()), pnlError, LocalizeString("TemplateError.Text"), false);
                Exceptions.LogException(Ex);
            }
            
        }


        #region Show Message or Errors on THIS DNN-Page
        protected void ShowMessage(string Message, PlaceHolder pnlMessage, bool ShowOnlyEdit = true)
		{
			if (!ShowOnlyEdit || UserMayEditThisModule)
			{
				pnlMessage.Controls.Add(new LiteralControl(Message));
				pnlMessage.Visible = true;
			}
		}

		protected void ShowError(string Error, Panel pnlError, string NonHostError = "", bool ShowOnlyEdit = true)
		{
			if (String.IsNullOrEmpty(NonHostError))
				NonHostError = Error;

			if (!ShowOnlyEdit || UserMayEditThisModule)
			{
				pnlError.Controls.Add(new LiteralControl(UserInfo.IsSuperUser ? Error : NonHostError));
				pnlError.Visible = true;
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
						return new ModuleActionCollection();

					if (_ModuleActions == null)
					{
						_ModuleActions = new ModuleActionCollection();
						var actions = _ModuleActions;

						if (ZoneId.HasValue && AppId.HasValue)
						{
							if (!Template?.UseForList ?? false)
							{
								//if (Template != null)
								//{
									// Edit item
							    actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif",
							        "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
							        SecurityAccessLevel.Edit, true, false);
							    //}
							}


							if (Template != null && Template.UseForList)
							{
								// Add Item
								actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true, false);
							}

							// Change layout button
							actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false, SecurityAccessLevel.Edit, true, false);
						}

						if (!SecurityHelpers.SexyContentDesignersGroupConfigured(PortalId) || SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo))
						{
							if (ZoneId.HasValue && AppId.HasValue && Template != null)
							{
                                // Edit Template Button
                                //Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", ModuleId.ToString(), "TemplateID", Template.TemplateId.ToString()), false, SecurityAccessLevel.Admin, true, true);
                                actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test", true,
                                        SecurityAccessLevel.Edit, true, false);
                            }

							// App management
							if (ZoneId.HasValue && AppId.HasValue)
                                actions.Add(GetNextActionID(), "Admin" + (_sxcInstance.IsContentApp ? "" : " " + _sxcInstance.App.Name), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                                        SecurityAccessLevel.Admin, true, false);

                            // Zone management (app list)
                            if (!_sxcInstance.IsContentApp)
                                actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
                                        SecurityAccessLevel.Admin, true, false);
                        }
					}
					return _ModuleActions;
				}
				catch(Exception e)
				{
					Exceptions.LogException(e);
					return new ModuleActionCollection();
				}
			}
		}

        #endregion

        #endregion


    }
}