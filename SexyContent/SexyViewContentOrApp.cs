using System;
using System.Linq;
using System.Threading;
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
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.Security;
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

			// If logged in, inject Edit JavaScript, and delete / add items
		    if (UserMayEditThisModule)
		        try
		        {
		            RegisterClientDependencies();
		            var clientInfos = InfosForTheClientScripts();
		            ((ModuleHost) Parent).Attributes.Add("data-2sxc", JsonConvert.SerializeObject(clientInfos));
		        }
		        catch (Exception ex)
		        {
		            Exceptions.ProcessModuleLoadException(this, ex);
		        }
		}

	    private object InfosForTheClientScripts()
	    {
	        var hasContent = AppId.HasValue && Template != null && ContentGroup.Exists;

	        // minor workaround because the settings in the cache are wrong after using a page template
	        var tempVisibleStatus = DnnStuffToRefactor.TryToGetReliableSetting(ModuleConfiguration,
	            ToSic.SexyContent.Settings.SettingsShowTemplateChooser);
	        var templateChooserVisible = bool.Parse(tempVisibleStatus ?? "true");

	        var languages =
	            ZoneHelpers.GetCulturesWithActiveState(PortalId, ZoneId.Value)
	                .Where(c => c.Active)
	                .Select(c => new {key = c.Code.ToLower(), name = c.Text});

	        var priLang = PortalSettings.DefaultLanguage.ToLower(); // primary language 

	        // for now, don't filter by existing languages, this causes side-effects in many cases. 
	        //if (!languages.Where(l => l.key == priLang).Any())
	        //    priLang = "";

	        var clientInfos = new
	        {
	            moduleId = ModuleId,
	            manage = new
	            {
	                isEditMode = UserMayEditThisModule,
	                templateChooserVisible,
	                hasContent,
	                isContentApp = IsContentApp,
	                zoneId = ZoneId ?? 0,
	                appId = AppId,
	                isList = AppId.HasValue && ContentGroup.Content.Count > 1,
	                templateId = Template?.TemplateId,
	                contentTypeId = Template?.ContentTypeStaticName ?? "",
	                config = new
	                {
	                    portalId = PortalId,
	                    tabId = TabId,
	                    moduleId = ModuleId,
	                    contentGroupId = AppId.HasValue ? ContentGroup.ContentGroupGuid : (Guid?) null,
	                    dialogUrl = Globals.NavigateURL(TabId),
	                    returnUrl = Request.RawUrl,
	                    appPath = AppId.HasValue ? SxcContext.App.Path + "/" : null,
	                    // 2016-02-27 2dm - seems unused
	                    //cultureDimension = AppId.HasValue ? Sexy.GetCurrentLanguageID() : new int?(),
	                    isList = Template != null && Template.UseForList,
	                    version = ToSic.SexyContent.Settings.Version.ToString() // SexyContent.Version.ToString()
	                },
	                user = new
	                {
	                    canDesign = SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo),
	                    // will be true for admins or for people in the designers-group
	                    canDevelop = UserInfo.IsSuperUser // will be true for host-users, false for all others
	                },
	                applicationRoot = ResolveUrl("~"),
	                lang = PortalSettings.CultureCode.ToLower(),
	                //System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower(),
	                langPrimary = priLang,
	                languages
	            }
	        };
	        return clientInfos;
	    }

	    private void RegisterClientDependencies()
	    {
	        var root = "~/desktopmodules/tosic_sexycontent/";
	        var ext = string.IsNullOrEmpty(Request.QueryString["debug"]) ? ".min.js" : ".js";

	        // add edit-mode CSS
	        ClientResourceManager.RegisterStyleSheet(Page, root + "dist/inpage/inpage.min.css");

	        // ToDo: Move these RegisterScripts to JS to prevent including AngularJS twice (from other modules)
	        ClientResourceManager.RegisterScript(Page, root + "js/angularjs/angular.min.js", 80);

	        // New: multi-language stuff
	        ClientResourceManager.RegisterScript(Page, root + "dist/lib/i18n/set.min.js");

	        ClientResourceManager.RegisterScript(Page, root + "js/2sxc.api" + ext, 90);
	        ClientResourceManager.RegisterScript(Page, root + "dist/inpage/inpage" + ext, 91);

	        ClientResourceManager.RegisterScript(Page, root + "js/angularjs/2sxc4ng" + ext, 93);
	        ClientResourceManager.RegisterScript(Page, root + "dist/config/config" + ext, 93);
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
					ShowError("Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>more</a>). Please click the following button to finish the upgrade:<br><a class='dnnPrimaryAction' href='?finishUpgrade=true'>Finish Upgrade</a>", pnlError, "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.", false);

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
				var toolbar = "<ul class='sc-menu' data-toolbar='" + JsonConvert.SerializeObject(new { sortOrder = 0, useModuleList = true, action = "edit" }) + "'></ul>";
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
				string renderedTemplate;

				var engine = EngineFactory.CreateEngine(Template);
			    var dataSource = SxcContext.DataSource;
				engine.Init(Template, SxcContext.App, ModuleConfiguration, dataSource, Request.QueryString["type"] == "data" ? InstancePurposes.PublishData : InstancePurposes.WebView, SxcContext);
				engine.CustomizeData();

				// Output JSON data if type=data in URL, otherwise render the template
				renderedTemplate = Request.QueryString["type"] == "data" ? StreamJsonToClient(dataSource) : engine.Render();

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


	    private string StreamJsonToClient(ViewDataSource dataSource)
	    {
	        string renderedTemplate;
	        if (dataSource.Publish.Enabled)
	        {
	            var publishedStreams = dataSource.Publish.Streams;
	            renderedTemplate = GetJsonFromStreams(dataSource,
	                publishedStreams.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
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
								Actions.Add(GetNextActionID(), LocalizeString("ActionAdd.Text"), "", "", "add.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").addItem();", true, SecurityAccessLevel.Edit, true, false);
							}

							// Change layout button
							Actions.Add(GetNextActionID(), LocalizeString("ActionChangeLayoutOrContent.Text"), "", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").changeLayoutOrContent();", false, SecurityAccessLevel.Edit, true, false);
						}

						if (!SecurityHelpers.SexyContentDesignersGroupConfigured(PortalId) || SecurityHelpers.IsInSexyContentDesignersGroup(UserInfo))
						{
							if (ZoneId.HasValue && AppId.HasValue && Template != null)
							{
                                // Edit Template Button
                                //Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", EditUrl(TabId, SexyContent.ControlKeys.EditTemplateFile, false, "mid", ModuleId.ToString(), "TemplateID", Template.TemplateId.ToString()), false, SecurityAccessLevel.Admin, true, true);
                                Actions.Add(GetNextActionID(), LocalizeString("ActionEditTemplateFile.Text"), ModuleActionType.EditContent, "templatehelp", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").develop();", "test", true,
                                        SecurityAccessLevel.Edit, true, false);
                            }

							// App management
							if (ZoneId.HasValue && AppId.HasValue)
                                Actions.Add(GetNextActionID(), "Admin" + (IsContentApp ? "" : " " + SxcContext.App.Name), "", "", "edit.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminApp();", "", true,
                                        SecurityAccessLevel.Admin, true, false);

                            // Zone management (app list)
                            if (!IsContentApp)
                                Actions.Add(GetNextActionID(), "Apps Management", "AppManagement.Action", "", "action_settings.gif", "javascript:$2sxcActionMenuMapper(" + ModuleId + ").adminZone();", "", true,
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

        #region JSON stuff

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        public string GetJsonFromStreams(IDataSource source, string[] streamsToPublish)
        {
            var language = Thread.CurrentThread.CurrentCulture.Name;

            var y = streamsToPublish.Where(k => source.Out.ContainsKey(k)).ToDictionary(k => k, s => new
            {
                List = (from c in source.Out[s].List select new DynamicEntity(c.Value, new[] { language }, SxcContext).ToDictionary() /*Sexy.ToDictionary(c.Value, language)*/).ToList()
            });

            return JsonConvert.SerializeObject(y);
        }


        #endregion
    }
}