using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Serializers;
using ToSic.SexyContent.WebApi.Permissions;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
	public class EntitiesController : SxcApiControllerBase
	{
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.2sEntC");
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);
            PerformSecurityCheck(appId, contentType, Grants.Read, context.Dnn.Module, context.App?.ZoneId);

            // note that the culture-code isn't actually used...
            return new EntityApi(appId, Log).GetOne(contentType, id, cultureCode);
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public dynamic GetManyForEditing([FromBody] List<ItemIdentifier> items, int appId)
        {
            Log.Add($"get many a#{appId}, items⋮{items.Count}");

            // to do full security check, we'll have to see what content-type is requested
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            permCheck.EnsureOrThrow(GrantSets.WriteSomething, items);

            permCheck.InitAppData();

            items = ConvertListIndexToId(items, permCheck.App);

            var list = new EntityApi(appId, Log).GetEntitiesForEditing(appId, items);

            // Reformat to the Entity-WithLanguage setup
            var listAsEwH = list.Select(p => new EntityWithHeaderOldFormat
            {
                Header = p.Header,
                Entity = p.Entity != null
                    ? EntityWithLanguages.Build(appId, p.Entity)
                    : null
            }).ToList();

            Log.Add($"will return items⋮{list.Count}");
            return listAsEwH;
        }

	    internal static List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> items, App app)
	    {
	        var newItems = new List<ItemIdentifier>();
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
	        return newItems;
	    }


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
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<EntityWithHeaderOldFormat> items, [FromUri] bool partOfPage = false)
        {
            // log and do security check
            Log.Add($"save many started with a#{appId}, i⋮{items.Count}, partOfPage:{partOfPage}");
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            permCheck.EnsureOrThrow(GrantSets.WriteSomething, items.Select(i => i.Header).ToList());
            permCheck.ThrowIfUserIsRestrictedAndFeatureNotEnabled();

            // list of saved IDs
            Dictionary<Guid, int> postSaveIds = null;

            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                var versioning = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                Log.Add("save with publishing");
                var context = GetContext(SxcInstance, Log);
                versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID, 
                    args => postSaveIds = SaveAndProcessGroups(permCheck.Permissions, appId, items, partOfPage));
            }
            else
            {
                Log.Add("save without publishing");
                postSaveIds = SaveAndProcessGroups(permCheck.Permissions, appId, items, partOfPage);
            }

            return postSaveIds;
        }

	    private Dictionary<Guid, int> SaveAndProcessGroups(PermissionCheckBase permChecker, int appId, List<EntityWithHeaderOldFormat> items, bool partOfPage)
	    {
	        var allowWriteLive = permChecker.UserMay(GrantSets.WritePublished);

            var forceDraft = !allowWriteLive;

            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed
	        var eavEntitiesController = new Eav.WebApi.EntitiesController(Log);
	        ((Serializer)eavEntitiesController.Serializer).Sxc = SxcInstance;
	        var ids = eavEntitiesController.SaveMany(appId, items, partOfPage, forceDraft);

	        Log.Add("check groupings");
	        // now assign all content-groups as needed
	        var groupItems = items.Where(i => i.Header.Group != null)
	            .GroupBy(i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add)
	            .ToList();

	        // if it's new, it has to be added to a group
	        // only add if the header wants it, AND we started with ID unknown
	        if (groupItems.Any())
	            DoAdditionalGroupProcessing(SxcInstance, Log, appId, ids, groupItems);

	        return ids;
	    }

        private static void DoAdditionalGroupProcessing(SxcInstance sxcInstance, Log log, int appId, Dictionary<Guid, int> postSaveIds, IEnumerable<IGrouping<string, EntityWithHeaderOldFormat>> groupItems)
        {
            var myLog = new Log("2Ap.GrpPrc", log, "start");
            var app = new App(new DnnTenant(PortalSettings.Current), appId);
            var userMayEdit = sxcInstance.UserMayEdit ;

            app.InitData(userMayEdit, sxcInstance.Environment.PagePublishing.IsEnabled(sxcInstance.EnvInstance.Id /*ActiveModule.ModuleID*/), sxcInstance.Data.ConfigurationProvider);

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

                var postSaveId = postSaveIds[contItem.Entity.Guid];

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
            sxcInstance.ContentBlock.Manager.UpdateTitle();
        }

	    /// <summary>
	    /// Get all Entities of specified Type
	    /// </summary>
	    [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId,
	        string cultureCode = null)
	        => new EntityApi(appId, Log).GetEntities(contentType, cultureCode);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
            // check if admin rights, then ok
	        var context = GetContext(SxcInstance, Log);
	        var zoneId = context.App?.ZoneId;
            PerformSecurityCheck(appId, contentType, Grants.Read, context.Dnn.Module, zoneId);

            return new EntityApi(appId, Log).GetEntitiesForAdmin(contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);
            PerformSecurityCheck(appId, contentType, Grants.Delete, context.Dnn.Module, context.App?.ZoneId);

            new EntityApi(appId, Log).Delete(contentType, id, force);
        }
        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);
            PerformSecurityCheck(appId, contentType, Grants.Delete, context.Dnn.Module, context.App?.ZoneId);
            new EntityApi(appId, Log).Delete(contentType, guid, force);
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
	        return new AppManager(appId, Log).Entities.VersionHistory(item.EntityId);
            //return EavEntitiesController.History(appId, item.EntityId);
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
            new AppManager(appId, Log).Entities.VersionRestore(item.EntityId, changeId);
	        return true;

            //return EavEntitiesController.Restore(appId, item.EntityId, changeId);
	    }
        #endregion
    }
}
