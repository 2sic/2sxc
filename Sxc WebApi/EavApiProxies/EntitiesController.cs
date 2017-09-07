using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.Serializers;
using System.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Security;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Eav.Apps.Enums;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	public class EntitiesController : SxcApiController 
	{
	    private readonly Eav.WebApi.EntitiesController _entitiesController = new Eav.WebApi.EntitiesController();

	    private void EnsureSerializerHasSxc()
	    {
            (_entitiesController.Serializer as Serializer).Sxc = SxcContext;
	    }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            // todo: try to refactor into the other "GetOne"
            // note that the culture-code isn't actually used...
            EnsureSerializerHasSxc();

            // check if admin rights, then ok
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, useContext: true, appId: appId);

            return _entitiesController.GetOne(contentType, id, appId, cultureCode);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic GetManyForEditing([FromBody] List<ItemIdentifier> items, int appId)
        {
            // this will contain the list of the items we'll really return
            var newItems = new List<ItemIdentifier>();

            // go through all the groups, assign relevant info so that we can then do get-many
            foreach (var reqItem in items)
            {
                // only do special processing if it's a "group" item
                if (reqItem.Group == null)
                {
                    newItems.Add(reqItem);
                    continue;
                }
                var app = new App(PortalSettings.Current, appId);
                app.InitData(SxcContext.Environment.Permissions.UserMayEditContent, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(this.ActiveModule.ModuleID), Data.ConfigurationProvider);
                
                var contentGroup = app.ContentGroupManager.GetContentGroup(reqItem.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(reqItem.Group.Part);

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                var part = contentGroup[reqItem.Group.Part];
                reqItem.ContentTypeName = contentTypeStaticName;
                if (!reqItem.Group.Add && // not in add-mode
                              part.Count > reqItem.Group.Index && // has as many items as desired
                              part[reqItem.Group.Index] != null) // and the slot has something
                    reqItem.EntityId = part[reqItem.Group.Index].EntityId;

                // tell the UI that it should not actually use this data yet, keep it locked
                if (reqItem.Group.Part.ToLower().Contains(AppConstants.PresentationLower)) {
                    reqItem.Group.SlotCanBeEmpty = true;  // all presentations can always be locked
                    if (reqItem.EntityId == 0)
                    {
                        reqItem.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with
                        
                        reqItem.DuplicateEntity =
                            reqItem.Group.Part.ToLower() == AppConstants.PresentationLower
                            ? contentGroup.Template.PresentationDemoEntity?.EntityId
                            : contentGroup.Template.ListPresentationDemoEntity?.EntityId;
                    }
                }
                
                newItems.Add(reqItem);
            }

            // Now get all
            return _entitiesController.GetManyForEditing(appId, newItems);

            // todo: find out how to handle Presentation items
        }

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        // todo: should refactor to save all items in 1 transaction
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeader> items, [FromUri] bool partOfPage = false)
        {
            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            // var allowAddGroup = items.First().Entity.Id == 0;

            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed

            var versioning = new PagePublishing();
            Dictionary<Guid, int> postSaveIds = null;

            Action<Eav.Apps.Environment.VersioningActionInfo> internalSave = (args) => {
                postSaveIds = _entitiesController.SaveMany(appId, items, partOfPage);
                
                // now assign all content-groups as needed
                var groupItems = items
                    .Where(i => i.Header.Group != null)
                    .GroupBy(i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add)
                    .ToList();

                if (groupItems.Any())
                    DoAdditionalGroupProcessing(appId, postSaveIds, groupItems);
            };

            // use dnn versioning if partOfPage
            if (partOfPage) versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, internalSave);
            else internalSave(null);

            return postSaveIds;
        }

        private void DoAdditionalGroupProcessing(int appId, Dictionary<Guid, int> postSaveIds, IEnumerable<IGrouping<string, EntityWithHeader>> groupItems)
        {
            var app = new App(PortalSettings.Current, appId);
            app.InitData(SxcContext.Environment.Permissions.UserMayEditContent, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(this.ActiveModule.ModuleID), Data.ConfigurationProvider);

            foreach (var entitySets in groupItems)
            {
                var contItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ContentLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListContentLower);// "listcontent");
                if (contItem == null)
                    throw new Exception("unexpected group-entity assigment, cannot figure it out");

                var presItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.PresentationLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListPresentationLower);// "listpresentation");

                // Get group to assign to and parameters
                var contentGroup = app.ContentGroupManager.GetContentGroup(contItem.Header.Group.Guid);
                var partName = contItem.Header.Group.Part;

                // var part = contentGroup[partName];
                var index = contItem.Header.Group.Index;

                // Get saved entity (to get its ID)
                if (!postSaveIds.ContainsKey(contItem.Entity.Guid))
                    throw new Exception("Saved entity not found - not able to update ContentGroup");

                int postSaveId = postSaveIds[contItem.Entity.Guid];

                int? presentationId = null;

                if (presItem != null)
                {
                    if (postSaveIds.ContainsKey(presItem.Entity.Guid))
                        presentationId = postSaveIds[presItem.Entity.Guid];

                    presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId;
                    // use null if it shouldn't have one
                }
                // add or update slots
                var reallyAddGroup = contItem.Entity.Id == 0; // only really add if it's really new
                if (contItem.Header.Group.Add && reallyAddGroup) // this cannot be auto-detected, it must be specified
                    contentGroup.AddContentAndPresentationEntity(partName, index, postSaveId, presentationId);
                else // if (part.Count <= index || part[index] == null)
                    contentGroup.UpdateEntityIfChanged(partName, index, postSaveId, true, presentationId);
            }

            // update-module-title
            SxcContext.ContentBlock.Manager.UpdateTitle();
        }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null)
		{
            EnsureSerializerHasSxc();
			return _entitiesController.GetEntities(contentType, cultureCode, appId);
		}

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
            EnsureSerializerHasSxc();

            // check if admin rights, then ok
            PerformSecurityCheck(contentType, PermissionGrant.Read, true, useContext: true, appId: appId);

            return _entitiesController.GetAllOfTypeForAdmin(appId, contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            EnsureSerializerHasSxc();

            // check if admin rights, then ok
            PerformSecurityCheck(contentType, PermissionGrant.Delete, true, useContext: true, appId: appId);

            _entitiesController.Delete(contentType, id, appId, force);
        }
        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            EnsureSerializerHasSxc();

            // check if admin rights, then ok
            PerformSecurityCheck(contentType, PermissionGrant.Delete, true, useContext: true, appId: appId);

            _entitiesController.Delete(contentType, guid, appId, force);
        }


	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public Dictionary<string, object> CreateOrUpdate(string contentType, int id = 0)
	    {
	        throw new NotImplementedException();
	    }

        #region Content Types
        /// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IContentType GetContentType(string contentType, int appId)
		{
            EnsureSerializerHasSxc();

            return new Eav.WebApi.ContentTypeController().GetSingle(appId, contentType, null);
            // return entitesController.GetContentType(contentType, appId);
		}

        #endregion

        #region versioning

	    //[HttpGet]
     //   [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
     //   public List<ItemHistory> History(int appId, int entityId)
	    //{
     //       EnsureSerializerHasSxc();
	    //    return _entitiesController.History(appId, entityId);
	    //}

	    //[HttpGet]
     //   [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
     //   public bool Restore(int appId, int entityId, int changeId)
	    //{
     //       EnsureSerializerHasSxc();
	    //    return _entitiesController.Restore(appId, entityId, changeId);
	    //}

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> History(int appId, [FromBody]ItemIdentifier item)
	    {
            EnsureSerializerHasSxc();
	        ResolveItemIdOfGroup(appId, item);
            return _entitiesController.History(appId, item.EntityId);
        }

	    private static void ResolveItemIdOfGroup(int appId, ItemIdentifier item)
	    {
            if (item.Group == null) return;
	        var app = new App(PortalSettings.Current, appId);
	        var contentGroup = app.ContentGroupManager.GetContentGroup(item.Group.Guid);
	        var part = contentGroup[item.Group.Part];
	        item.EntityId = part[item.Group.Index].EntityId;
	    }

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody]ItemIdentifier item)
	    {
            EnsureSerializerHasSxc();
            ResolveItemIdOfGroup(appId, item);
            return _entitiesController.Restore(appId, item.EntityId, changeId);
	    }
        #endregion
    }
}
