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
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Installer;
using ToSic.Eav.Apps.ItemListActions;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Interfaces;
using Assembly = System.Reflection.Assembly;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi.View
{
    // had to disable this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {
        private ContentGroupReferenceManagerBase _cbm;

        private ContentGroupReferenceManagerBase ContentGroupReferenceManager => _cbm ?? (_cbm = SxcContext.ContentBlock.Manager);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void AddItem([FromUri] int? sortOrder = null, [FromUri] bool partOfPage = false)
        {
            var versioning = new PagePublishing();

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                ContentGroupReferenceManager.AddItem(sortOrder);
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup, bool? newTemplateChooserState = null)
            => ContentGroupReferenceManager.SaveTemplateId(templateId, forceCreateContentGroup, newTemplateChooserState);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetTemplateChooserState([FromUri] bool state) => ContentGroupReferenceManager.SetTemplateChooserState(state);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId) => ContentGroupReferenceManager.SetAppId(appId);

        #region Get Apps, ContentTypes and Templates for UI

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> GetSelectableApps() => ContentGroupReferenceManager.GetSelectableApps();

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> GetSelectableContentTypes() => ContentGroupReferenceManager.GetSelectableContentTypes();


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> GetSelectableTemplates() => ContentGroupReferenceManager.GetSelectableTemplates();

        #endregion

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            var contentTypeName = Settings.AttributeSetStaticNameContentBlockTypeName;
            var values = new Dictionary<string, object>
            {
                {EntityContentBlock.CbPropertyTitle, ""},
                {EntityContentBlock.CbPropertyApp, app},
                {EntityContentBlock.CbPropertyShowChooser, true},
            };
            var newGuid = guid ?? Guid.NewGuid();
            var entityId = CreateItemAndAddToList(parentId, field, sortOrder, contentTypeName, values, newGuid);

            // now return a rendered instance
            var newContentBlock = new EntityContentBlock(SxcContext.ContentBlock, entityId);
            return newContentBlock.SxcInstance.Render().ToString();

        }

        private int CreateItemAndAddToList(int parentId, string field, int sortOrder, string contentTypeName,
            Dictionary<string, object> values, Guid newGuid)
        {
            var cgApp = SxcContext.App;

            // create the new entity 
            var entityId = new AppManager(cgApp).Entities.GetOrCreate(newGuid, contentTypeName, values);

            #region attach to the current list of items

            var cbEnt = SxcContext.App.Data["Default"].List[parentId];
            var blockList = ((Eav.Data.EntityRelationship) cbEnt.GetBestValue(field))?.ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (sortOrder > intList.Count) sortOrder = intList.Count;
                intList.Insert(sortOrder, entityId);
            }
            var updateDic = new Dictionary<string, object> {{field, intList}};
            new AppManager(cgApp).Entities.UpdateParts(cbEnt.EntityId, updateDic);
            #endregion
            
            return entityId;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void MoveItemInList(int parentId, string field, int indexFrom, int indexTo, [FromUri] bool partOfPage = false)
        {
            var versioning = new PagePublishing();

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                ModifyItemList(parentId, field, new Move(indexFrom, indexTo));
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);
        }

        /// <summary>
        /// 2016-04-07 2dm: note: remove was never tested! UI not clear yet
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="field"></param>
        /// <param name="index"></param>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveItemInList(int parentId, string field, int index, [FromUri] bool partOfPage = false)
        {
            var versioning = new PagePublishing();

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                ModifyItemList(parentId, field, new Remove(index));
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang, bool cbIsEntity = false)
        {
            try
            {
                // Try setting thread language to enable 2sxc to render the template in this language
                if (!string.IsNullOrEmpty(lang))
                    try
                    {
                        var culture = System.Globalization.CultureInfo.GetCultureInfo(lang);
                        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    }
                    // Fallback / ignore if the language specified has not been found
                    catch (System.Globalization.CultureNotFoundException) { }

                var cbToRender = SxcContext.ContentBlock;

                // if a real templateid was specified, swap to that
                if (templateId > 0)
                {
                    var template = cbToRender.App.TemplateManager.GetTemplate(templateId);
                    cbToRender.SxcInstance.Template = template;
                }

                var rendered = cbToRender.SxcInstance.Render().ToString();

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                };

            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw;
            }
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void ChangeOrder([FromUri] int sortOrder, int destinationSortOrder, [FromUri] bool partOfPage = false)
        {
            var versioning = new PagePublishing();

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                ContentGroupReferenceManager.ChangeOrder(sortOrder, destinationSortOrder);
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int sortOrder) => ContentGroupReferenceManager.Publish(part, sortOrder);
        
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(int id) => ContentGroupReferenceManager.Publish(id, true);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void RemoveFromList([FromUri] int sortOrder, [FromUri] bool partOfPage = false)
        {
            var versioning = new PagePublishing();

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                ContentGroupReferenceManager.RemoveFromList(sortOrder);
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public string RemoteInstallDialogUrl(string dialog, bool isContentApp)
        {
            // note / warning: some duplicate code with SystemController.cs
            
            if (dialog != "gettingstarted")
                throw new Exception("unknown dialog name: " + dialog);


            var moduleInfo = Request.FindModuleInfo();
            var modName = moduleInfo.DesktopModule.ModuleName;

            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (isContentApp)
            {
                // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                try
                {
                    var all = SxcContext.App.TemplateManager /*.AppTemplates*/.GetAllTemplates();
                    if (all.Any())
                        return null;
                }
                catch
                {
                    // ignored
                }
            }

            // Add desired destination
            // Add DNN Version, 2SexyContent Version, module type, module id, Portal ID
            var gettingStartedSrc = "//gettingstarted.2sxc.org/router.aspx?"
                + "destination=autoconfigure" + (isContentApp ? Eav.Constants.ContentAppName.ToLower() : "app")
                + "&DnnVersion=" + Assembly.GetAssembly(typeof(Globals)).GetName().Version.ToString(4)
                + "&2SexyContentVersion=" + Settings.ModuleVersion
                + "&ModuleName=" + modName + "&ModuleId=" + moduleInfo.ModuleID
                + "&PortalID=" + moduleInfo.PortalID;
            // Add VDB / Zone ID (if set)
            var zoneId = Env.ZoneMapper.GetZoneId(moduleInfo.PortalID);
            gettingStartedSrc +=  "&ZoneID=" + zoneId;

            // Add DNN Guid
            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
            // Add Portal Default Language & current language
            gettingStartedSrc += "&DefaultLanguage=" + PortalSettings.DefaultLanguage
                + "&CurrentLanguage=" + PortalSettings.CultureCode;

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
            var ic = new InstallationController();
            if (ic.IsUpgradeRunning)
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 10 minutes, please restart the web application.");

            ic.FinishAbortedUpgrade();

            return true;
        }

        #region Helpers to get things done
        // todo: probably should move to the new Eav.Apps section, but for that we must
        // change the access to use the DB, not the cache...
        private bool ModifyItemList(int parentId, string field, IItemListAction actionToPerform)
        {
            var parentEntity = SxcContext.App.Data["Default"].List[parentId];
            var parentField = parentEntity.GetBestValue(field);
            var fieldList = parentField as Eav.Data.EntityRelationship;

            if (fieldList == null)
                throw new Exception("field " + field + " doesn't seem to be a list of content-items, must abort");

            var ids = fieldList.EntityIds.ToList();

            if (!actionToPerform.Change(ids)) return false;

            // save
            var values = new Dictionary<string, object> { { field, ids.ToArray() } };
            new AppManager(SxcContext.App).Entities.UpdateParts(parentEntity.EntityId, values);
            return true;
        }
        #endregion
    }
}