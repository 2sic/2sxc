using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Serializers;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	public class EntitiesController : SxcApiController
	{
	    private Eav.WebApi.EntitiesController EavEntitiesController { get; set; }
 
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sEntC");
            EavEntitiesController = new Eav.WebApi.EntitiesController(Log);
	        ((Serializer)EavEntitiesController.Serializer).Sxc = SxcInstance;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            // check if admin rights, then ok
            PerformSecurityCheck(appId, contentType, PermissionGrant.Read, Dnn.Module, App);

            // note that the culture-code isn't actually used...
            return EavEntitiesController.GetOne(contentType, id, appId, cultureCode);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public dynamic GetManyForEditing([FromBody] List<ItemIdentifier> items, int appId)
        {
            Log.Add($"get many a#{appId}, items⋮{items.Count}");
            // this will contain the list of the items we'll really return
            var newItems = new List<ItemIdentifier>();

            // go through all the groups, assign relevant info so that we can then do get-many
            var app = GetAppForWritingOrThrow(appId);

            // todo: change to only show drafts if permissions allow more than just create

            var showDrafts = true;

            app.InitData(showDrafts, 
                SxcInstance.Environment.PagePublishing.IsEnabled(ActiveModule.ModuleID), 
                Data.ConfigurationProvider);

            foreach (var reqItem in items)
            {
                // only do special processing if it's a "group" item
                if (reqItem.Group == null)
                {
                    newItems.Add(reqItem);
                    continue;
                }
                
                var contentGroup = app.ContentGroupManager.GetContentGroup(reqItem.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(reqItem.Group.Part);

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                ConvertListIndexToEntityIds(contentGroup, reqItem, contentTypeStaticName);

                newItems.Add(reqItem);
            }

            // Now get all
            return EavEntitiesController.GetManyForEditing(appId, newItems);
        }

	    //private App GetAppAndCheckWriteSecurity(int appId)
	    //{
	    //    var env = Factory.Resolve<IEnvironmentFactory>().Environment(Log);
	    //    var tenant = new DnnTenant(PortalSettings.Current);
	    //    var uiZoneId = env.ZoneMapper.GetZoneId(tenant.Id);

	    //    // now do relevant security checks
	    //    var app = GetAppForWritingOrThrow(appId, tenant, uiZoneId);
	    //    return app;
	    //}


	    private static void ConvertListIndexToEntityIds(ContentGroup contentGroup, ItemIdentifier reqItem,
	        string contentTypeStaticName)
	    {
	        var part = contentGroup[reqItem.Group.Part];
	        reqItem.ContentTypeName = contentTypeStaticName;
	        if (!reqItem.Group.Add && // not in add-mode
	            part.Count > reqItem.Group.Index && // has as many items as desired
	            part[reqItem.Group.Index] != null) // and the slot has something
	            reqItem.EntityId = part[reqItem.Group.Index].EntityId;

	        // tell the UI that it should not actually use this data yet, keep it locked
	        if (!reqItem.Group.Part.ToLower().Contains(AppConstants.PresentationLower))
                return;

	        reqItem.Group.SlotCanBeEmpty = true; // all presentations can always be locked

	        if (reqItem.EntityId != 0)
                return;

	        reqItem.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with

	        reqItem.DuplicateEntity =
	            reqItem.Group.Part.ToLower() == AppConstants.PresentationLower
	                ? contentGroup.Template.PresentationDemoEntity?.EntityId
	                : contentGroup.Template.ListPresentationDemoEntity?.EntityId;
	    }

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeader> items, [FromUri] bool partOfPage = false)
        {
            // start with full security check
            var app = GetAppForWritingOrThrow(appId);

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown


            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed

            Log.Add($"save many started with a#{appId}, i⋮{items.Count}, partOfPage:{partOfPage}");

            var versioning = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
            Dictionary<Guid, int> postSaveIds = null;

            void InternalSave(VersioningActionInfo args)
            {
                postSaveIds = EavEntitiesController.SaveMany(appId, items, partOfPage);

                Log.Add("check groupings");
                // now assign all content-groups as needed
                var groupItems = items.Where(i => i.Header.Group != null)
                    .GroupBy(i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add)
                    .ToList();

                if (groupItems.Any())
                    DoAdditionalGroupProcessing(appId, postSaveIds, groupItems);
            }

            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                Log.Add("save with publishing");
                versioning.DoInsidePublishing(Dnn.Module.ModuleID, Dnn.User.UserID, InternalSave);
            }
            else InternalSave(null);

            return postSaveIds;
        }

        private void DoAdditionalGroupProcessing(int appId, Dictionary<Guid, int> postSaveIds, IEnumerable<IGrouping<string, EntityWithHeader>> groupItems)
        {
            var myLog = new Log("2Ap.GrpPrc", Log, "start");
            var app = new App(new DnnTenant(PortalSettings.Current), appId);
            var userMayEdit = SxcInstance.UserMayEdit ;// Factory.Resolve<IPermissions>().UserMayEditContent(SxcInstance.InstanceInfo);

            app.InitData(userMayEdit, SxcInstance.Environment.PagePublishing.IsEnabled(ActiveModule.ModuleID), Data.ConfigurationProvider);

            foreach (var entitySets in groupItems)
            {
                myLog.Add("processing:" + entitySets.Key);
                var contItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ContentLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListContentLower);
                if (contItem == null)
                    throw new Exception("unexpected group-entity assigment, cannot figure it out");

                var presItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.PresentationLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListPresentationLower);

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
            SxcInstance.ContentBlock.Manager.UpdateTitle();
        }

        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId, string cultureCode = null) 
            => EavEntitiesController.GetEntities(contentType, cultureCode, appId);

	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
            // check if admin rights, then ok
            PerformSecurityCheck(appId, contentType, PermissionGrant.Read, Dnn.Module, App);

            return EavEntitiesController.GetAllOfTypeForAdmin(appId, contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            // check if admin rights, then ok
            PerformSecurityCheck(appId, contentType, PermissionGrant.Delete, Dnn.Module, App);

            EavEntitiesController.Delete(contentType, id, appId, force);
        }
        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            // check if admin rights, then ok
            PerformSecurityCheck(appId, contentType, PermissionGrant.Delete, Dnn.Module, App);

            EavEntitiesController.Delete(contentType, guid, appId, force);
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
        public ContentTypeInfo GetContentType(string contentType, int appId) 
            => new Eav.WebApi.ContentTypeController().GetSingle(appId, contentType, null);

	    #endregion

        #region versioning

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> History(int appId, [FromBody]ItemIdentifier item)
	    {
            ResolveItemIdOfGroup(appId, item);
            return EavEntitiesController.History(appId, item.EntityId);
        }

	    private static void ResolveItemIdOfGroup(int appId, ItemIdentifier item)
	    {
            if (item.Group == null) return;
	        var app = new App(new DnnTenant(PortalSettings.Current), appId);
	        var contentGroup = app.ContentGroupManager.GetContentGroup(item.Group.Guid);
	        var part = contentGroup[item.Group.Part];
	        item.EntityId = part[item.Group.Index].EntityId;
	    }

	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Restore(int appId, int changeId, [FromBody]ItemIdentifier item)
	    {
            ResolveItemIdOfGroup(appId, item);
            return EavEntitiesController.Restore(appId, item.EntityId, changeId);
	    }
        #endregion
    }
}
