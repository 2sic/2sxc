using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.ViewAPI
{
    // had to disable this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {
        private ContentBlockManagerBase _cbm;
        private ContentBlockManagerBase ContentBlockManager => _cbm ?? (_cbm = SxcContext.ContentBlock.Manager);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? sortOrder = null)
        {
            ContentBlockManager.AddItem(sortOrder);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
        {
            return ContentBlockManager.SaveTemplateId(templateId, forceCreateContentGroup, newTemplateChooserState);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetTemplateChooserState([FromUri] bool state)
		{
            ContentBlockManager.SetTemplateChooserState(state);
		}

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableApps()
		{
		    return ContentBlockManager.GetSelectableApps();
		}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId)
        {
            ContentBlockManager.SetAppId(appId);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableContentTypes()
        {
            return ContentBlockManager.GetSelectableContentTypes();
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableTemplates()
        {
            return ContentBlockManager.GetSelectableTemplates();
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang, bool cbIsEntity = false, int cbid = 0)
        {
            try
            {
                // Try setting thread language to enable 2sxc to render the template in this language
                if (!String.IsNullOrEmpty(lang))
                    try
                    {
                        var culture = System.Globalization.CultureInfo.GetCultureInfo(lang);
                        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    }
                    // Fallback / ignore if the language specified has not been found
                    catch (System.Globalization.CultureNotFoundException) { }

                var cbToRender = !cbIsEntity 
                    ? SxcContext.ContentBlock 
                    : new EntityContentBlock(SxcContext.ContentBlock, cbid);
                var template = cbToRender.App.TemplateManager.GetTemplate(templateId);
                cbToRender.SxcInstance.Template = template;

                var rendered = cbToRender.SxcInstance.Render().ToString();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                };

            }
            catch (RenderingException e)
            {
                if (e.RenderStatus == RenderStatusType.MissingData)
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(EngineBase.ToolbarForEmptyTemplate)
                    };
				Exceptions.LogException(e);
                throw e;

            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw e;
            }
        }

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder)
		{
            ContentBlockManager.ChangeOrder(sortOrder, destinationSortOrder);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int sortOrder)
        {
            return ContentBlockManager.Publish(part, sortOrder);

        }

        [HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList([FromUri] int sortOrder)
		{
            ContentBlockManager.RemoveFromList(sortOrder);
		}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
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

        /// <summary>
        /// Finish system installation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        // had to disable this, as most requests now come from a lone page [ValidateAntiForgeryToken]
        public bool FinishInstallation()
        {
            if (Installer.IsUpgradeRunning)
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 10 minutes, please restart the web application.");

            Installer.FinishAbortedUpgrade();

            return true;
        }
    }
}