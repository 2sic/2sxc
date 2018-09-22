using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Serializers;
using ToSic.SexyContent.WebApi.Permissions;
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
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
                throw exception;
            // 2018-09-15 old code, should have checked the same stuff mostly...
            //new AppPermissionBeforeUsing(SxcInstance, Log)
            //    .ConfirmPermissionsOrThrow(contentType, appId, Grants.Read);
            return new EntityApi(appId, Log).GetOne(contentType, id, cultureCode);  // note that the culture-code isn't actually used...
        }


        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public dynamic GetManyForEditing([FromBody] List<ItemIdentifier> items, int appId)
        {
            var wrapLog = Log.Call("GetManyForEditing", $"get many a#{appId}, items⋮{items.Count}");

            // before we start, we have to convert the indexes into something more useful, because
            // otherwise in content-list scenaries we don't have the type
            var appForSecurityChecks = App.LightWithoutData(new DnnTenant(PortalSettings), appId, Log);
            items = new SaveHelpers.ContentGroupList(SxcInstance, Log).ConvertListIndexToId(items, appForSecurityChecks);

            // to do full security check, we'll have to see what content-type is requested
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, items, Log);

            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;
            // 2018-09-15 old code, should have checked the same stuff mostly...
            //if (!permCheck.Ensure(GrantSets.WriteSomething, /*items,*/ out var exp))
            //    throw exp;

            var list = new EntityApi(appId, Log).GetEntitiesForEditing(appId, items);

            // Reformat to the Entity-WithLanguage setup
            var listAsEwH = list.Select(p => new BundleWithHeader<EntityWithLanguages>
            {
                Header = p.Header,
                Entity = p.Entity != null
                    ? EntityWithLanguages.Build(appId, p.Entity)
                    : null
            }).ToList();

            wrapLog($"will return items⋮{list.Count}");
            return listAsEwH;
        }



	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<BundleWithHeader<EntityWithLanguages>> items, [FromUri] bool partOfPage = false)
        {
            // log and do security check
            Log.Add($"save many started with a#{appId}, i⋮{items.Count}, partOfPage:{partOfPage}");
            var permCheck = new SaveHelpers.Security(SxcInstance, Log).DoSaveSecurityCheck(appId, items);

            return new SaveHelpers.DnnPublishing(SxcInstance, Log)
                .SaveWithinDnnPagePublishing(appId, items, partOfPage,
                    forceSaveAsDraft => SaveOldFormatKeepTillReplaced(appId, items, partOfPage, forceSaveAsDraft),
                    permCheck);
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
	        return eavEntitiesController.SaveMany(appId, items, partOfPage, forceDraft);
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
	        var permCheck = new MultiPermissionsTypes(SxcInstance, appId, contentType, Log);
	        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
	            throw exception;
	        // 2018-09-15 old code, should have checked the same stuff mostly...
	        //new AppPermissionBeforeUsing(SxcInstance, Log)
         //       .ConfirmPermissionsOrThrow(contentType, appId, Grants.Read);
            return new EntityApi(appId, Log).GetEntitiesForAdmin(contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.DeleteSomething, out var exception))
                throw exception;
            // 2018-09-15 old code, should have checked the same stuff mostly...
            //new AppPermissionBeforeUsing(SxcInstance, Log)
            //    .ConfirmPermissionsOrThrow(contentType, appId, GrantSets.DeleteSomething);
            new EntityApi(appId, Log).Delete(contentType, id, force);
        }

	    [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.DeleteSomething, out var exception))
                throw exception;
            // 2018-09-15 old code, should have checked the same stuff mostly...
            //new AppPermissionBeforeUsing(SxcInstance, Log)
            //    .ConfirmPermissionsOrThrow(contentType, appId, GrantSets.DeleteSomething);
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
	        var app = App.LightWithoutData(new DnnTenant(PortalSettings.Current), appId, null);
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
