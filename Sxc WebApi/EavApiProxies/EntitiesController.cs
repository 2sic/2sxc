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
using Guid = System.Guid;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
	/// <inheritdoc />
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
            var zaId = new AppIdentity(context.App.ZoneId, appId, Log);
            PerformSecurityCheck(zaId, contentType, Grants.Read, context.Dnn.Module);

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
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, items, out var exp))
                throw exp;

            permCheck.InitAppData();

            items = ConvertListIndexToId(items, permCheck.App);

            var list = new EntityApi(appId, Log).GetEntitiesForEditing(appId, items);

            // Reformat to the Entity-WithLanguage setup
            var listAsEwH = list.Select(p => new BundleWithHeader<EntityWithLanguages>
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
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<BundleWithHeader<EntityWithLanguages>> items, [FromUri] bool partOfPage = false)
        {
            // log and do security check
            Log.Add($"save many started with a#{appId}, i⋮{items.Count}, partOfPage:{partOfPage}");
            var permCheck = SaveHelpers.Security.DoSaveSecurityCheck(SxcInstance, appId, items, Log);

            Dictionary<Guid, int> SaveOldWithGroups(bool forceSaveAsDraft) => 
                SaveOldFormatKeepTillReplaced(appId, items, partOfPage, forceSaveAsDraft);

            return SaveHelpers.DnnPublishing
                .SaveWithinDnnPagePublishing(SxcInstance, appId, items, partOfPage, 
                SaveOldWithGroups, permCheck, Log);
        }






	    private Dictionary<Guid, int> SaveOldFormatKeepTillReplaced(int appId, 
            List<BundleWithHeader<EntityWithLanguages>> items, 
            bool partOfPage,
            bool forceDraft)
	    {
	        Log.Add($"SaveAndProcessGroups(..., appId:{appId}, items:{items?.Count}), partOfPage:{partOfPage}");

            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed
	        var eavEntitiesController = new Eav.WebApi.EntitiesController(Log);
	        ((Serializer)eavEntitiesController.Serializer).Sxc = SxcInstance;
	        var ids = eavEntitiesController.SaveMany(appId, items, partOfPage, forceDraft);

	        // now assign all content-groups as needed
            //new SaveHelpers.ContentGroup(SxcInstance, Log)
            //    .DoGroupProcessingIfNecessary(appId, items, ids);

	        return ids;
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
            //   // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);

            var zaId = GetAppIdentity(context, appId, PortalSettings.UserInfo.IsSuperUser);

            PerformSecurityCheck(zaId, contentType, Grants.Read, context.Dnn.Module);

            return new EntityApi(zaId.AppId, Log).GetEntitiesForAdmin(contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);
            var zaId = new AppIdentity(context.App.ZoneId, appId, Log);
            PerformSecurityCheck(zaId, contentType, Grants.Delete, context.Dnn.Module);

            new EntityApi(appId, Log).Delete(contentType, id, force);
        }
        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            // check if admin rights, then ok
            var context = GetContext(SxcInstance, Log);
            var zaId = new AppIdentity(context.App.ZoneId, appId, Log);
            PerformSecurityCheck(zaId, contentType, Grants.Delete, context.Dnn.Module);
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
	    }
        #endregion
    }
}
