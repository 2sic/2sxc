using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Client.ClientResourceManagement;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;

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
					ClientScriptManager ClientScript = Page.ClientScript;
					// ToDo: Move these RegisterScripts to JS to prevent including AngularJS twice (from other modules)
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/AngularJS/angular.min.js", 80);
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.TemplateSelector.js", 81);
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.ApiService.js", 82);
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/ViewEdit.js", 82);
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.DnnActionMenuMapper.js", 83);

					var hasContent = AppId.HasValue && Template != null && Elements.Any(p => p.EntityId.HasValue);
					var templateChooserVisible = Settings.ContainsKey(SexyContent.SettingsShowTemplateChooser) ?
						Boolean.Parse(Settings[SexyContent.SettingsShowTemplateChooser].ToString())
						: !hasContent;

					((DotNetNuke.UI.Modules.ModuleHost)this.Parent).Attributes.Add("data-2sxc", Newtonsoft.Json.JsonConvert.SerializeObject(new
					{
						moduleId = ModuleId,
						manage = new
						{
							isEditMode = UserMayEditThisModule,
							templateChooserVisible = templateChooserVisible,
							hasContent = hasContent,
							isContentApp = IsContentApp,
							appId = AppId,
							isList = AppId.HasValue && Elements.Count > 1,
							templateId = Template != null ? Template.TemplateID : new int?(),
							config = new
							{
								portalId = PortalId,
								tabId = TabId,
								moduleId = ModuleId,
								contentGroupId = AppId.HasValue ? Sexy.GetContentGroupIdFromModule(ModuleId) : 0,
								dialogUrl = DotNetNuke.Common.Globals.NavigateURL(this.TabId),
								returnUrl = Request.RawUrl,
								appPath = AppId.HasValue ? Sexy.App.Path : null,
								cultureDimension = AppId.HasValue ? Sexy.GetCurrentLanguageID() : new int?(),
								isList = Template != null && Template.UseForList
							}
						}
					}));

					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.js", 90);
					ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/ToSIC_SexyContent/Js/2sxc.api.manage.js", 91);
				}
			}
			catch (Exception ex)
			{
				Exceptions.ProcessModuleLoadException(this, ex);
			}

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

			if (Template.AttributeSetID.HasValue && DataSource.GetCache(ZoneId.Value, AppId.Value).GetContentType(Template.AttributeSetID.Value) == null)
			{
				ShowError("The contents of this module cannot be displayed because it's located in another VDB.", pnlError);
				return;
			}

			if (Template.AttributeSetID.HasValue && Elements.All(e => e.Content == null))
			{
				var toolbar = IsEditable ? "<ul class='sc-menu' data-toolbar='" + Newtonsoft.Json.JsonConvert.SerializeObject(new { sortOrder = Elements.First().SortOrder, useModuleList = true, action = "edit" }) + "'></ul>" : "";
				ShowMessage(LocalizeString("NoDemoItem.Text") + " " + toolbar, pnlMessage);
				return;
			}

			#endregion

			try
			{
				//var renderTemplate = Template;
				string renderedTemplate;

				var engine = EngineFactory.CreateEngine(Template);
				var dataSource = (ViewDataSource)Sexy.GetViewDataSource(this.ModuleId, SexyContent.HasEditPermission(this.ModuleConfiguration), Template);
				engine.Init(Template, Sexy.App, this.ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView, Sexy);
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
						renderedTemplate = Newtonsoft.Json.JsonConvert.SerializeObject(new { error = "2sxc Content (" + ModuleId + "): " + String.Format(LocalizeString("EnableDataPublishing.Text"), ModuleId, moduleTitle) });
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
				if (ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID)
					return new ModuleActionCollection();

				if (_ModuleActions == null)
				{
					_ModuleActions = new ModuleActionCollection();
					ModuleActionCollection Actions = _ModuleActions;

					try
					{
						if (ZoneId.HasValue && AppId.HasValue)
						{
							if (!IsList)
							{
								if (Template != null)
								{
									// Edit item
									Actions.Add(GetNextActionID(), LocalizeString("ActionEdit.Text"), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").edit();", "test", true,
										SecurityAccessLevel.Edit, true, false);
								}
							}
							else
							{
								// Edit List
								Actions.Add(GetNextActionID(), LocalizeString("ActionList.Text"),
									ModuleActionType.ContentOptions, "editlist", "edit.gif",
									EditUrl(this.TabId, SexyContent.ControlKeys.EditList, false, "mid",
										this.ModuleId.ToString(), SexyContent.ContentGroupIDString,
										Elements.First().GroupID.ToString()), false,
									SecurityAccessLevel.Edit, true, false);
							}

							if (Template != null && Template.UseForList)
							{
								// Add Item
								Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true, false);
							}

							// Change layout button
							Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + this.ModuleId + ").changeLayoutOrContent();", false, SecurityAccessLevel.Edit, true, false);
						}

						if (!SexyContent.SexyContentDesignersGroupConfigured(PortalId) || SexyContent.IsInSexyContentDesignersGroup(UserInfo))
						{
							if (ZoneId.HasValue && AppId.HasValue && Template != null)
							{
								// Edit Template Button
								Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(this.TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", this.ModuleId.ToString(), "TemplateID", Template.TemplateID.ToString()), false, SecurityAccessLevel.Admin, true, true);
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
					catch (Exception e)
					{
						Exceptions.ProcessModuleLoadException(this, e);
					}
				}
				return _ModuleActions;
			}
		}

		#endregion
	}
}