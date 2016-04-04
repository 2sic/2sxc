using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Eav.BLL;
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
        private ContentGroupReferenceManagerBase _cbm;
        private ContentGroupReferenceManagerBase ContentGroupReferenceManager => _cbm ?? (_cbm = SxcContext.ContentBlock.Manager);


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? sortOrder = null) 
            => ContentGroupReferenceManager.AddItem(sortOrder);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
            => ContentGroupReferenceManager.SaveTemplateId(templateId, forceCreateContentGroup, newTemplateChooserState);
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetTemplateChooserState([FromUri] bool state)
		    => ContentGroupReferenceManager.SetTemplateChooserState(state);
		

		[HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableApps()
		    => ContentGroupReferenceManager.GetSelectableApps();
		

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId)
            => ContentGroupReferenceManager.SetAppId(appId);
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableContentTypes()
            => ContentGroupReferenceManager.GetSelectableContentTypes();
        

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<object> GetSelectableTemplates()
            => ContentGroupReferenceManager.GetSelectableTemplates();

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "")
        {
            var cgApp = SxcContext.App;
            var context = EavDataController.Instance(cgApp.ZoneId, cgApp.AppId).Entities;

            #region create the new entity --> note that it's the sql-type entity, not a standard ientity
            var contentType = DataSource.GetCache(cgApp.ZoneId, cgApp.AppId).GetContentType(Settings.AttributeSetStaticNameContentBlockTypeName);
            var values = new Dictionary<string, object>
            {
                {EntityContentBlock.CbPropertyTitle, ""},
                {EntityContentBlock.CbPropertyApp, app},
                {EntityContentBlock.CbPropertyShowChooser, true},
            };

            var entity = context.AddEntity(contentType.AttributeSetId, values, null, null);
            #endregion

            #region attach to the current list of items

            var cbEnt = SxcContext.App.Data["Default"].List[parentId]; // ((EntityContentBlock) SxcContext.ContentBlock).ContentBlockEntity;
            var blockList = ((Eav.Data.EntityRelationship)cbEnt.GetBestValue(field)).ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            intList.Insert(sortOrder, entity.EntityID);

            var updateDic = new Dictionary<string, int[]>();
            updateDic.Add(field, intList.ToArray());

            context.UpdateEntity(cbEnt.EntityGuid, updateDic);
            #endregion

            // now return a rendered instance
            var newContentBlock = new EntityContentBlock(SxcContext.ContentBlock, entity.EntityID);
            return newContentBlock.SxcInstance.Render().ToString();

        }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang, bool cbIsEntity = false)
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

                var cbToRender = SxcContext.ContentBlock;
                var template = cbToRender.App.TemplateManager.GetTemplate(templateId);
                cbToRender.SxcInstance.Template = template;

                var rendered = cbToRender.SxcInstance.Render().ToString();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                };

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
            ContentGroupReferenceManager.ChangeOrder(sortOrder, destinationSortOrder);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int sortOrder)
        {
            return ContentGroupReferenceManager.Publish(part, sortOrder);

        }

        [HttpGet]
		[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList([FromUri] int sortOrder)
		{
            ContentGroupReferenceManager.RemoveFromList(sortOrder);
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