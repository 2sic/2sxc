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
using ToSic.SexyContent.ContentBlocks;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Internal;
using Assembly = System.Reflection.Assembly;

namespace ToSic.SexyContent.WebApi.View
{
    // had to disable this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {
        private ContentGroupReferenceManagerBase _cbm;

        private ContentGroupReferenceManagerBase ContentGroupReferenceManager
            => _cbm ?? (_cbm = SxcContext.ContentBlock.Manager);


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
        public string GenerateContentBlock(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            var contentTypeName = Settings.AttributeSetStaticNameContentBlockTypeName;
            var values = new Dictionary<string, object>
            {
                {EntityContentBlock.CbPropertyTitle, ""},
                {EntityContentBlock.CbPropertyApp, app},
                {EntityContentBlock.CbPropertyShowChooser, true},
            };

            var entityId = CreateItemAndAddToList(parentId, field, sortOrder, contentTypeName, values, guid);

            // now return a rendered instance
            var newContentBlock = new EntityContentBlock(SxcContext.ContentBlock, entityId);
            return newContentBlock.SxcInstance.Render().ToString();

        }

        private int CreateItemAndAddToList(int parentId, string field, int sortOrder, string contentTypeName,
            Dictionary<string, object> values, Guid? newGuid)
        {
            var cgApp = SxcContext.App;
            // 2017-04-01 2dm centralizing eav access
            var bridge = new EavBridge(cgApp);
            //var eavDc = EavDataController.Instance(cgApp.ZoneId, cgApp.AppId);
            //eavDc.UserName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;

            #region create the new entity --> note that it's the sql-type entity, not a standard ientity

            //var contentType =
            //    DataSource.GetCache(cgApp.ZoneId, cgApp.AppId)
            //        .GetContentType(contentTypeName);

            int entityId;
            // check that it doesn't exist yet...
            if (newGuid.HasValue && bridge.EntityExists(newGuid.Value)) // eavDc.Entities.EntityExists(newGuid.Value))
                entityId = bridge.EntityGetOrResurect(newGuid.Value);
            //{
            //    // check if it's deleted - if yes, resurrect
            //    var existingEnt = eavDc.Entities.GetEntitiesByGuid(newGuid.Value).First();
            //    if (existingEnt.ChangeLogDeleted != null)
            //        existingEnt.ChangeLogDeleted = null;

            //    entityId = existingEnt.EntityID;
            //}
            else
                entityId = bridge.EntityCreate(contentTypeName, values, entityGuid: newGuid).Item1;
            //{
            //    var entity = eavDc.Entities.AddEntity(contentType.AttributeSetId, values, null, null, entityGuid: newGuid);
            //    entityId = entity.EntityID;
            //}

            #endregion

            #region attach to the current list of items

            var cbEnt = SxcContext.App.Data["Default"].List[parentId];
            // ((EntityContentBlock) SxcContext.ContentBlock).ContentBlockEntity;
            var blockList = ((Eav.Data.EntityRelationship) cbEnt.GetBestValue(field)).ToList() ?? new List<IEntity>();

            var intList = blockList.Select(b => b.EntityId).ToList();
            // add only if it's not already in the list (could happen if http requests are run again)
            if (!intList.Contains(entityId))
            {
                if (sortOrder > intList.Count) sortOrder = intList.Count;
                intList.Insert(sortOrder, entityId);
            }
            var updateDic = new Dictionary<string, object> {{field, intList.ToArray()}};
            bridge.EntityUpdate(cbEnt.EntityId, updateDic);
            //eavDc.Entities.UpdateEntity(cbEnt.EntityGuid, updateDic);

            #endregion

            return entityId;
        }

        //public bool MoveContentBlock(int parentId, string field, int indexFrom, int indexTo)
        //{
        //    MoveItemInList(parentId, field, indexFrom, indexTo);

        //    return true;
        //}

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool MoveItemInList(int parentId, string field, int indexFrom, int indexTo)
        {
            var action = new MoveItem(indexFrom, indexTo);
            ModifyItemList(parentId, field, action);
            return true;
        }

        /// <summary>
        /// 2016-04-07 2dm: note: remove was never tested! UI not clear yet
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="field"></param>
        /// <param name="index"></param>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool RemoveItemInList(int parentId, string field, int index)
        {
            var action = new RemoveItem(index);
            ModifyItemList(parentId, field, action);
            return true;
        }


        private void ModifyItemList(int parentId, string field, IItemListAction actionToPerform)
        {
            var parentEntity = SxcContext.App.Data["Default"].List[parentId];

            var parentField = parentEntity.GetBestValue(field);
            var fieldList = parentField as Eav.Data.EntityRelationship;

            if (fieldList == null)
                throw new Exception("field " + field + " doesn't seem to be a list of content-items, must abort");

            var ids = fieldList.EntityIds.ToList();

            if (!actionToPerform.Change(ids)) return;

            // save
            var values = new Dictionary<string, object> {{field, ids.ToArray()}};
            // 2017-04-01 2dm centralizing eav access
            new EavBridge(SxcContext.App)
                .EntityUpdate(parentEntity.EntityId, values);
            //var cgApp = SxcContext.App;
            //var eavDc = EavDataController.Instance(cgApp.ZoneId, cgApp.AppId);
            //eavDc.UserName = Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;
            //eavDc.Entities.UpdateEntity(parentEntity.EntityGuid, values);
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
            => ContentGroupReferenceManager.Publish(part, sortOrder);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(int id)
            => ContentGroupReferenceManager.Publish(id, true);
    

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
            // new: check if it should allow this
            // it should only be allowed, if the current situation is either
            // Content - and no views exist (even invisible ones)
            // App - and no apps exist - this is already checked on client side, so I won't include a check here
            if (isContent)
            {
                // we'll usually run into errors if nothing is installed yet, so on errors, we'll continue
                try
                {
                    var all = SxcContext.App.TemplateManager /*.AppTemplates*/.GetAllTemplates();
                    if (all.Any())
                        return null;
                }
                catch { }
            }

            var gettingStartedSrc = "//gettingstarted.2sxc.org/router.aspx?";

            // Add desired destination
            gettingStartedSrc += "destination=autoconfigure" + (isContent ? Constants.ContentAppName.ToLower() : "app");

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
            var zoneId = Env.ZoneMapper.GetZoneId(moduleInfo.PortalID);// ZoneHelpers.GetZoneId(moduleInfo.PortalID);
            gettingStartedSrc +=  "&ZoneID=" + zoneId;

            // Add DNN Guid
            var hostSettings = HostController.Instance.GetSettingsDictionary();
            gettingStartedSrc += hostSettings.ContainsKey("GUID") ? "&DnnGUID=" + hostSettings["GUID"] : "";
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
            var ic = new InstallationController();
            if (ic.IsUpgradeRunning)
                throw new Exception("There seems to be an upgrade running - please wait. If you still see this message after 10 minutes, please restart the web application.");

            ic.FinishAbortedUpgrade();

            return true;
        }
    }

    internal interface IItemListAction
    {
        bool Change(List<int?> ids);
    }

    internal class MoveItem : IItemListAction
    {
        private int _indexFrom, _indexTo;
        public MoveItem(int from, int to)
        {
            _indexFrom = from;
            _indexTo = to;
        }
        public bool Change(List<int?> ids)
        {
            if (_indexFrom >= ids.Count) // this is if you set cut after the last item
                _indexFrom = ids.Count - 1;
            if (_indexTo >= ids.Count)
                _indexTo = ids.Count;
            if (_indexFrom == _indexTo)
                return false;

            // do actualy re-ordering
            var oldId = ids[_indexFrom];
            ids.RemoveAt(_indexFrom);
            if (_indexTo > _indexFrom) _indexTo--; // the actual index could have shifted due to the removal
            ids.Insert(_indexTo, oldId);
            return true;

        }
    }

    internal class RemoveItem : IItemListAction
    {
        private int _index;
        public RemoveItem(int index)
        {
            _index = index;
        }
        public bool Change(List<int?> ids)
        {
            // don't allow rmove outside of index
            if (_index < 0 || _index >= ids.Count) 
                return false;

            // do actualy re-ordering
            ids.RemoveAt(_index);
            return true;

        }
    }



}