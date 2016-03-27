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
	public class SexyViewContentOrApp : SexyControlEditBase
	{

        protected void Page_Load(object sender, EventArgs e)
		{
            // always do this, part of the guarantee that everything will work
			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            var renderHelp = new RenderingHelpers(_sxcInstance, ModuleContext, SettingsAreStored, ResolveUrl("~"));


			// If logged in, inject Edit JavaScript, and delete / add items
            if (!UserMayEditThisModule) return;
            try
            {
                // register scripts and css
                renderHelp.RegisterClientDependencies(Page, string.IsNullOrEmpty(Request.QueryString["debug"]));

                // new
                var newSpecs = new ClientInfosAll(ResolveUrl("~"), PortalSettings, ModuleContext, _sxcInstance, UserInfo,
                    ZoneId ?? 0, SettingsAreStored);
                ((ModuleHost) Parent).Attributes.Add("data-2sxc-context", JsonConvert.SerializeObject(newSpecs));

                // add instance specific infos to the html-tag
                var clientInfos = renderHelp.InfosForTheClientScripts();
                ((ModuleHost) Parent).Attributes.Add("data-2sxc", JsonConvert.SerializeObject(clientInfos));

                // Add some required variables to module host div which are general global 2sxc-infos
                var globalInfos = renderHelp.RegisterGlobalsAttribute();
                ((ModuleHost)Parent).Attributes.Add("data-2sxc-globals", JsonConvert.SerializeObject(globalInfos));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
		}


        private SxcInstance _sxcInstanceForSecurityChecks;
        protected bool UserMayEditThisModule // todo: unsure if this works, generating the new sxc without appid = 0
        {
            get
            {
                if (_sxcInstanceForSecurityChecks == null)
                    _sxcInstanceForSecurityChecks = _sxcInstance 
                        ?? new ModuleContentBlock(ModuleConfiguration, UserInfo, Request.QueryString, false).SxcInstance ;// new SxcInstance(ZoneId.Value, 0, ModuleConfiguration);
                return _sxcInstanceForSecurityChecks?.Environment?.Permissions.UserMayEditContent ?? false;
            }
        }

        private string GetRenderedTemplateOrJson()
	    {
            // note that initializing the engine will also initialize any custom data-changes
            // so this is needed, even if we will only render to JSON afterwards
	        var engine = _sxcInstance.GetRenderingEngine(Request.QueryString["type"] == "data"
	            ? InstancePurposes.PublishData
	            : InstancePurposes.WebView);

	        // Output JSON data if type=data in URL, otherwise render the template
	        return Request.QueryString["type"] == "data"
	            ? StreamJsonToClient(_sxcInstance.Data)
	            : engine.Render();
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

            #region  2016-03-01 old stuff, delete later when stable...
            //try
            //{
            //    var engine =
            //        SxcContext.RenderingEngine(Request.QueryString["type"] == "data"
            //            ? InstancePurposes.PublishData
            //            : InstancePurposes.WebView);
            //             //   var dataSource = SxcContext.DataSource;
            //             //var engine = EngineFactory.CreateEngine(Template);
            //             //engine.Init(Template, SxcContext.App, ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView, SxcContext);
            //             //engine.CustomizeData(); // this is also important for json-output

            //             // Output JSON data if type=data in URL, otherwise render the template
            //             var renderedTemplate = Request.QueryString["type"] == "data" ? StreamJsonToClient(SxcContext.DataSource) : engine.Render();
            //             //renderedTemplate = Request.QueryString["type"] == "data" ? StreamJsonToClient(dataSource) : engine.Render();

            //	// If standalone is specified, output just the template without anything else
            //	if (StandAlone)
            //	{
            //		Response.Clear();
            //		Response.Write(renderedTemplate);
            //		Response.Flush();
            //		Response.SuppressContent = true;
            //		HttpContext.Current.ApplicationInstance.CompleteRequest();
            //	}
            //	else
            //		phOutput.Controls.Add(new LiteralControl(renderedTemplate));
            //}
            //// Catch errors; log them
            //catch (Exception Ex)
            //{
            //	ShowError(LocalizeString("TemplateError.Text") + ": " + HttpUtility.HtmlEncode(Ex.ToString()), pnlError, LocalizeString("TemplateError.Text"), false);
            //	Exceptions.LogException(Ex);
            //}
            #endregion
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
							else
							{
                                // Removed edit list - does not work yet
								//// Edit List
								//Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"),
								//	ModuleActionType.ContentOptions, "editlist", "edit.gif",
								//	EditUrl(TabId, SexyContent.ControlKeys.EditList, false, "mid",
								//		ModuleId.ToString()), false,
								//	SecurityAccessLevel.Edit, true, false);
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
                                actions.Add(GetNextActionID(), "Admin" + (IsContentApp ? "" : " " + _sxcInstance.App.Name), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                                        SecurityAccessLevel.Admin, true, false);

                            // Zone management (app list)
                            if (!IsContentApp)
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


        // todo: move outside of this system...
        //protected string CheckUpgradeMessage()
        //{
        //    // Upgrade success check - show message if upgrade did not run successfully
        //    if (SexyContentModuleUpgrade.UpgradeComplete) return null;// || !UserInfo.IsSuperUser) return true;

        //    return SexyContentModuleUpgrade.IsUpgradeRunning
        //        ? "It looks like a 2sxc upgrade is currently running.Please wait for the operation to complete(the upgrade may take a few minutes)."
        //        : UserInfo.IsSuperUser
        //            ? "Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>more</a>). Please click the following button to finish the upgrade: <br><a class='dnnPrimaryAction' onclick='$2sxc.system.finishUpgrade(this)'>Finish Upgrade</a>"
        //            : "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.";
        //}

    }
}