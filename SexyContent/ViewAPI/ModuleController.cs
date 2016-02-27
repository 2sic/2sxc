using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Statics;
using ToSic.SexyContent.WebApi;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.ViewAPI
{
    [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void AddItem([FromUri] int? sortOrder = null)
        {
			var contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ActiveModule.ModuleID);
			contentGroup.AddContentAndPresentationEntity("content", sortOrder, null, null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
		public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
        {
            Guid? result = null;
            var contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ActiveModule.ModuleID);
            if (contentGroup.Exists || forceCreateContentGroup)
                result = Sexy.ContentGroups.SaveTemplateId(ActiveModule.ModuleID, templateId);
            else
                Sexy.ContentGroups.SetPreviewTemplateId(ActiveModule.ModuleID, templateId);

            if(newTemplateChooserState.HasValue)
                SetTemplateChooserState(newTemplateChooserState.Value);

            return result;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
		public void SetTemplateChooserState([FromUri] bool state)
		{
            DnnStuffToRefactor.UpdateModuleSettingForAllLanguages(ActiveModule.ModuleID, Settings.SettingsShowTemplateChooser, state.ToString());

			//new DotNetNuke.Entities.Modules.ModuleController().UpdateModuleSetting(ActiveModule.ModuleID,
			//	SexyContent.SettingsShowTemplateChooser, state.ToString());
		}

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		[ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableApps()
        {
            try
            {
                var zoneId = ZoneHelpers.GetZoneID(ActiveModule.PortalID);
				return
					AppHelpers.GetApps(zoneId.Value, false, new PortalSettings(ActiveModule.OwnerPortalID))
                        .Where(a => !a.Hidden)
						.Select(a => new {a.Name, a.AppId});
            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw e;
            }
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public void SetAppId(int? appId)
        {
            AppHelpers.SetAppIdForModule(ActiveModule, appId);
            }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableContentTypes()
        {
			return Sexy.Templates.GetAvailableContentTypesForVisibleTemplates().Select(p => new {p.StaticName, p.Name});
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public IEnumerable<object> GetSelectableTemplates()
        {
            var availableTemplates = Sexy.Templates.GetAvailableTemplatesForSelector(ActiveModule.ModuleID, Sexy.ContentGroups);
			return availableTemplates.Select(t => new {t.TemplateId, t.Name, t.ContentTypeStaticName});
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
		public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang)
        {
            try
            {
                // Try setting thread language to enable 2sxc to render the template in this language
                if(!String.IsNullOrEmpty(lang))
                {
                    try
                    {
                        var culture = System.Globalization.CultureInfo.GetCultureInfo(lang);
                        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    }
                    catch (System.Globalization.CultureNotFoundException) // Fallback if the language specified has not been found
                    { }
                }
                

				var template = Sexy.Templates.GetTemplate(templateId);

                var engine = EngineFactory.CreateEngine(template);
                // before 2016-02-27 2dm: 
                //var dataSource =
                //	(ViewDataSource)
                //		Sexy.GetViewDataSource(ActiveModule.ModuleID, SecurityHelpers.HasEditPermission(ActiveModule), template);
                var dataSource =
					(ViewDataSource) ViewDataSource.ForModule(ActiveModule.ModuleID, SecurityHelpers.HasEditPermission(ActiveModule), template, Sexy);
                engine.Init(template, Sexy.App, ActiveModule, dataSource, InstancePurposes.WebView, Sexy);
                engine.CustomizeData();

				if (template.ContentTypeStaticName != "" && template.ContentDemoEntity == null &&
				    !dataSource["Default"].List.Any())// .Count == 0)
				{
					var toolbar = "<ul class='sc-menu' data-toolbar='" +
					              JsonConvert.SerializeObject(new {sortOrder = 0, useModuleList = true, action = "edit"}) + "'></ul>";
					return new HttpResponseMessage(HttpStatusCode.OK)
					{
						Content =
							new StringContent("<div class='dnnFormMessage dnnFormInfo'>No demo item exists for the selected template. " +
							                  toolbar + "</div>")
					};
				}

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent(engine.Render(), Encoding.UTF8, "text/plain");
                return response;
            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw e;
            }
        }

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		[ValidateAntiForgeryToken]
		public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder)
		{
			try
			{
				var contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ActiveModule.ModuleID);
				contentGroup.ReorderEntities(sortOrder, destinationSortOrder);
			}
			catch (Exception e)
			{
				Exceptions.LogException(e);
				throw;
			}
		}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [ValidateAntiForgeryToken]
        public bool Publish(string part, int sortOrder)
        {
            try
            {
                var contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ActiveModule.ModuleID);
                var contEntity = contentGroup[part][sortOrder];
                var presKey = part.ToLower() == "content" ? "presentation" : "listpresentation";
                var presEntity = contentGroup[presKey][sortOrder];

                var hasPresentation = presEntity != null;

                // make sure we really have the draft item an not the live one
                var contDraft = contEntity.IsPublished ? contEntity.GetDraft() : contEntity;
                if (contEntity != null && !contDraft.IsPublished)
                    Sexy.ContentContext.Publishing.PublishDraftInDbEntity(contDraft.RepositoryId, !hasPresentation); // don't save yet if has pres...

                if (hasPresentation)
                {
                    var presDraft = presEntity.IsPublished ? presEntity.GetDraft() : presEntity;
                    if (!presDraft.IsPublished)
                        Sexy.ContentContext.Publishing.PublishDraftInDbEntity(presDraft.RepositoryId, true);
                }

                return true;
            }
            catch (Exception e)
            {
                Exceptions.LogException(e);
                throw;
            }
        }

        [HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
		[ValidateAntiForgeryToken]
		public void RemoveFromList([FromUri] int sortOrder)
		{
			try
			{
				var contentGroup = Sexy.ContentGroups.GetContentGroupForModule(ActiveModule.ModuleID);
				contentGroup.RemoveContentAndPresentationEntities("content", sortOrder);
			}
			catch (Exception e)
			{
				Exceptions.LogException(e);
				throw;
			}
		}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        [ValidateAntiForgeryToken]
        public string RemoteInstallDialogUrl(string dialog)
        {
            // note / warning: some duplicate code with SystemController.cs

            if (dialog != "gettingstarted")
                throw new Exception("unknown dialog name: " + dialog);

            var moduleInfo = Request.FindModuleInfo();
            var modName = moduleInfo.DesktopModule.ModuleName;

            var isContent = modName == "2sxc";
            var gettingStartedSrc = "//gettingstarted.2sxc.org/router.aspx?";

            // Add desired destination
            gettingStartedSrc += "destination=autoconfigure" + (isContent ? "content" : "app");

            // Add DNN Version
            gettingStartedSrc += "&DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4);
            // Add 2SexyContent Version
            gettingStartedSrc += "&2SexyContentVersion=" + Settings.ModuleVersion;
            // Add module type
            gettingStartedSrc += "&ModuleName=" + modName;
            // Add module id
            gettingStartedSrc += "&ModuleId=" + moduleInfo.ModuleID;
            // Add Portal ID
            gettingStartedSrc += "&PortalID=" + moduleInfo.PortalID;
            // Add VDB / Zone ID (if set)
            var ZoneID = ZoneHelpers.GetZoneID(moduleInfo.PortalID);
            gettingStartedSrc += ZoneID.HasValue ? "&ZoneID=" + ZoneID.Value : "";
            // Add AppStaticName and Version
            //if (App.AppId > 0 && !isContent)
            //{
            //    //var app =  SexyContent.GetApp(ZoneId.Value, AppId.Value, Sexy.OwnerPS);

            //    gettingStartedSrc += "&AppGuid=" + App.AppGuid;
            //    if (App.Configuration != null)
            //    {
            //        gettingStartedSrc += "&AppVersion=" + App.Configuration.Version;
            //        gettingStartedSrc += "&AppOriginalId=" + App.Configuration.OriginalId;
            //    }
            //}
            // Add DNN Guid
            var HostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += HostSettings.ContainsKey("GUID") ? "&DnnGUID=" + HostSettings["GUID"] : "";
            // Add Portal Default Language
            gettingStartedSrc += "&DefaultLanguage=" + PortalSettings.DefaultLanguage;
            // Add current language
            gettingStartedSrc += "&CurrentLanguage=" + PortalSettings.CultureCode;

            // Set src to iframe
            return gettingStartedSrc;
        }


    }
}