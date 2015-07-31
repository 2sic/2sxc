using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Security;

namespace ToSic.SexyContent
{
	/// <summary>
	/// This class contains code that are both used in ViewApp.ascx.cs and View.ascx.cs
	/// </summary>
	public class SexyViewContentOrApp : SexyControlEditBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{

			ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

			try
			{

				// If logged in, inject Edit JavaScript, and delete / add items
				if (UserMayEditThisModule)
				{
					// ToDo: Move these RegisterScripts to JS to prevent including AngularJS twice (from other modules)
                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/angularjs/angular.min.js", 80);
                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/template-selector/template-selector.min.js", 81);

                    // New: multi-language stuff
                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/bower_components/angular-translate/angular-translate.min.js", 82);
                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/bower_components/angular-translate-loader-static-files/angular-translate-loader-static-files.min.js", 83);

                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/dnn-inpage-edit.min.js", 82);

                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/2sxc.api.min.js", 90);
                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/2sxc.api.manage.min.js", 91);

                    ClientResourceManager.RegisterScript(Page, "~/desktopmodules/tosic_sexycontent/js/angularjs/2sxc4ng.min.js", 93); 

					var hasContent = AppId.HasValue && Template != null && ContentGroup.Exists;
					var templateChooserVisible = Settings.ContainsKey(SexyContent.SettingsShowTemplateChooser) ?
						Boolean.Parse(Settings[SexyContent.SettingsShowTemplateChooser].ToString())
						: !hasContent;

					((ModuleHost)Parent).Attributes.Add("data-2sxc", JsonConvert.SerializeObject(new
					{
						moduleId = ModuleId,
						manage = new
						{
							isEditMode = UserMayEditThisModule, templateChooserVisible, hasContent,
							isContentApp = IsContentApp,
							appId = AppId,
							isList = AppId.HasValue && ContentGroup.Content.Count > 1,
							templateId = Template != null ? Template.TemplateId : new int?(),
							contentTypeId = Template != null ? Template.ContentTypeStaticName : "",
							config = new
							{
								portalId = PortalId,
								tabId = TabId,
								moduleId = ModuleId,
								contentGroupId = AppId.HasValue ? ContentGroup.ContentGroupGuid : (Guid?)null,
								dialogUrl = Globals.NavigateURL(TabId),
								returnUrl = Request.RawUrl,
								appPath = AppId.HasValue ? Sexy.App.Path : null,
								cultureDimension = AppId.HasValue ? Sexy.GetCurrentLanguageID() : new int?(),
								isList = Template != null && Template.UseForList
							},
                            lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower(),
                            fallbackLang = new [] {"en"},
                            applicationRoot = ResolveUrl("~")
						}
					}));

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

				if(SexyContentModuleUpgrade.IsUpgradeRunning)
					ShowError("It looks like a 2sxc upgrade is currently running. Please wait for the operation to complete (the upgrade may take a few minutes).", pnlError, "", false);
				else
					ShowError("Module upgrade did not complete successfully. Please click the following button to finish the upgrade:<br><a class='dnnPrimaryAction' href='?finishUpgrade=true'>Finish Upgrade</a>", pnlError);

				return false;
			}

			return true;
		}

		/// <summary>
		/// Get the content data and render it with the given template to the page.
		/// </summary>
		protected void ProcessView(PlaceHolder phOutput, Panel pnlError, Panel pnlMessage)
		{
			#region Check if everything has values and return if not

			if (Template == null)
			{
				ShowError(LocalizeString("TemplateConfigurationMissing.Text"), pnlError);
				return;
			}

			if (Template.ContentTypeStaticName != "" && DataSource.GetCache(ZoneId.Value, AppId.Value).GetContentType(Template.ContentTypeStaticName) == null)
			{
				ShowError("The contents of this module cannot be displayed because it's located in another VDB.", pnlError);
				return;
			}

			if (Template.ContentTypeStaticName != "" && Template.ContentDemoEntity == null && ContentGroup.Content.All(e => e == null))
			{
				var toolbar = IsEditable ? "<ul class='sc-menu' data-toolbar='" + JsonConvert.SerializeObject(new { sortOrder = 0, useModuleList = true, action = "edit" }) + "'></ul>" : "";
				ShowMessage(LocalizeString("NoDemoItem.Text") + " " + toolbar, pnlMessage);
				return;
			}

			#endregion

            #region PermissionsCheck
            // 2015-05-19 2dm: new: do security check if security exists
            // should probably happen somewhere else - so it doesn't throw errors when not even rendering...
            // maybe should show 
            var permissions = new Security.PermissionController(ZoneId.Value, AppId.Value, Template.Guid, this.ModuleContext.Configuration);

            // Views only need permissions to limit access, so only check if there are any configured permissions
            if (!UserInfo.IsInRole(PortalSettings.AdministratorRoleName) && permissions.PermissionList.Any())
                if (!permissions.UserMay(PermissionGrant.Read))
                    throw new UnauthorizedAccessException("This view is not accessible for the current user. To give access, change permissions in the view settings. See http://2sxc.org/help?tag=view-permissions");

            #endregion

            try
			{
				//var renderTemplate = Template;
				string renderedTemplate;

				var engine = EngineFactory.CreateEngine(Template);
				var dataSource = (ViewDataSource)Sexy.GetViewDataSource(ModuleId, SexyContent.HasEditPermission(ModuleConfiguration), Template);
				engine.Init(Template, Sexy.App, ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView, Sexy);
				engine.CustomizeData();

				// Output JSON data if type=data in URL
				if (Request.QueryString["type"] == "data")
				{
					if (dataSource.Publish.Enabled)
					{
						var publishedStreams = dataSource.Publish.Streams;
						renderedTemplate = Sexy.GetJsonFromStreams(dataSource, publishedStreams.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
					}
					else
					{
						Response.StatusCode = 403;
						var moduleTitle = new ModuleController().GetModule(ModuleId).ModuleTitle;
						renderedTemplate = JsonConvert.SerializeObject(new { error = "2sxc Content (" + ModuleId + "): " + String.Format(LocalizeString("EnableDataPublishing.Text"), ModuleId, moduleTitle) });
						Response.TrySkipIisCustomErrors = true;
					}
					Response.ContentType = "application/json";
				}
				else
				{
					renderedTemplate = engine.Render();
				}

				// If standalone is specified, output just the template without anything else
				if (StandAlone)
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
			// Catch errors; log them
			catch (Exception Ex)
			{
				ShowError(LocalizeString("TemplateError.Text") + ": " + HttpUtility.HtmlEncode(Ex.ToString()), pnlError, LocalizeString("TemplateError.Text"), false);
				Exceptions.LogException(Ex);
			}
		}

		#region Show Message or Errors
		protected void ShowMessage(string Message, Panel pnlMessage, bool ShowOnlyEdit = true)
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

		#region ModuleActions

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
						var Actions = _ModuleActions;

						if (ZoneId.HasValue && AppId.HasValue)
						{
							if (!IsList)
							{
								if (Template != null)
								{
									// Edit item
									Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").edit();", "test", true,
										SecurityAccessLevel.Edit, true, false);
								}
							}
							else
							{
								// Edit List
								Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"),
									ModuleActionType.ContentOptions, "editlist", "edit.gif",
									EditUrl(TabId, SexyContent.ControlKeys.EditList, false, "mid",
										ModuleId.ToString()), false,
									SecurityAccessLevel.Edit, true, false);
							}

							if (Template != null && Template.UseForList)
							{
								// Add Item
								Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true, false);
							}

							// Change layout button
							Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false, SecurityAccessLevel.Edit, true, false);
						}

						if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId) || SexyContent.IsInSexyContentDesignersGroup(UserInfo))
						{
							if (ZoneId.HasValue && AppId.HasValue && Template != null)
							{
								// Edit Template Button
								Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", ModuleId.ToString(), "TemplateID", Template.TemplateId.ToString()), false, SecurityAccessLevel.Admin, true, true);
							}

							// Administrator functions
							if (ZoneId.HasValue && AppId.HasValue)
								Actions.Add(GetNextActionID(), "Admin" + (IsContentApp ? "" : " " + Sexy.App.Name), "Admin.Action",
											"gettingstarted", "action_settings.gif", EditUrl("", "", "gettingstarted", SexyContent.AppIDString + "=" + AppId),
											false, SecurityAccessLevel.Admin, true, false);

							// App Management
							if (!IsContentApp)
								Actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action",
										"appmanagement", "action_settings.gif", EditUrl("", "", "appmanagement"),
										false, SecurityAccessLevel.Admin, true, false);
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
	}
}