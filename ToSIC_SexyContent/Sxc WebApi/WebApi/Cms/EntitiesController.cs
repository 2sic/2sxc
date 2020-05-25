using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Security;
using ToSic.Sxc.SxcTemp;
using Guid = System.Guid;

namespace ToSic.Sxc.WebApi.Cms
{
	/// <summary>
	/// Proxy Class to the EAV EntitiesController (Web API Controller)
	/// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
	public class EntitiesController : SxcApiControllerBase, IEntitiesController
	{
	    protected override void Initialize(HttpControllerContext controllerContext)
	    {
	        base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.2sEntC");
        }

	    /// <inheritdoc />
	    [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public Dictionary<string, object> GetOne(string contentType, int id, int appId, string cultureCode = null)
        {
            var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
                throw exception;
            return new EntityApi(appId, true, Log).GetOne(contentType, id, cultureCode);  // note that the culture-code isn't actually used...
        }

        #region Old API for V3 - Obsolete, must remove when we drop the old UI

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public dynamic GetManyForEditing([FromBody] List<ItemIdentifier> items, int appId)
        {
            var wrapLog = Log.Call($"get many a#{appId}, items⋮{items.Count}");

            // before we start, we have to convert the indexes into something more useful, because
            // otherwise in content-list scenarios we don't have the type
            var appForSecurityChecks = GetApp.LightWithoutData(new DnnTenant(PortalSettings), SystemRuntime.ZoneIdOfApp(appId), appId, Log);
            items = new ContentGroupList(BlockBuilder, Log).ConvertListIndexToId(items, appForSecurityChecks);

            // to do full security check, we'll have to see what content-type is requested
            var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, items, Log);

            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var exception))
                throw exception;

            var list = new EntityApi(appId, permCheck.EnsureAny(GrantSets.ReadDraft), Log).GetEntitiesForEditing(appId, items);

            // Reformat to the Entity-WithLanguage setup
            var listAsEwH = list.Select(p => new BundleWithHeader<EntityWithLanguages>
            {
                Header = p.Header,
                Entity = p.Entity != null
                    ? EntityWithLanguages.Build(appId, p.Entity)
                    : null
            }).ToList();

            // 2018-09-26 2dm
            // if we're giving items which already exist, then we must verify that edit/read is allowed.
            // important, this code is shared/duplicated in the UiController.Load
            if (list.Any(set => set.Entity != null))
                if (!permCheck.EnsureAll(GrantSets.ReadSomething, out exception))
                    throw exception;

            wrapLog($"will return items⋮{list.Count}");
            return listAsEwH;
        }


	    /// <inheritdoc />
	    [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> SaveMany([FromUri] int appId, [FromBody] List<BundleWithHeader<EntityWithLanguages>> items, [FromUri] bool partOfPage = false)
        {
            // log and do security check
            Log.Add($"save many started with a#{appId}, i⋮{items.Count}, partOfPage:{partOfPage}");

            var appReadForSecurityCheckOnly = new AppRuntime(appId, true, Log);
            #region check if it's an update, and do more security checks - shared with UiController.Save
            // basic permission checks
            var permCheck = new Security.Security(BlockBuilder, Log)
                .DoPreSaveSecurityCheck(appId, items);

            var foundItems = items.Where(i => i.EntityId != 0 && i.EntityGuid != Guid.Empty)
                .Select(i => i.EntityGuid != Guid.Empty
                    ? appReadForSecurityCheckOnly.Entities.Get(i.EntityGuid) // prefer guid access if available
                    : appReadForSecurityCheckOnly.Entities.Get(i.EntityId)  // otherwise id
                );
            if (foundItems.Any(i => i != null) && !permCheck.EnsureAll(GrantSets.UpdateSomething, out var exception))
                throw exception;
            #endregion

            return new DnnPublishing(BlockBuilder, Log)
                .SaveWithinDnnPagePublishingAndUpdateParent(appId, items, partOfPage,
                    forceSaveAsDraft => SaveOldFormatKeepTillReplaced(appId, items, partOfPage, forceSaveAsDraft),
                    permCheck);
        }



	    private Dictionary<Guid, int> SaveOldFormatKeepTillReplaced(int appId, 
            List<BundleWithHeader<EntityWithLanguages>> items, 
            bool partOfPage,
            bool forceDraft)
	    {
	        Log.Add($"SaveAndProcessGroups(..., appId:{appId}, items:{items?.Count}), partOfPage:{partOfPage}, forceDraft:{forceDraft}");

            // first, save all to do it in 1 transaction
            // note that it won't save the SlotIsEmpty ones, as these won't be needed
	        var eavEntitiesController = new Eav.WebApi.EntitiesController(Log);
	        return eavEntitiesController.SaveMany(appId, items, partOfPage, forceDraft);
	    }
        #endregion

        #region New feature in 11.03 - Usage Statitics

        public dynamic Usage(int appId, Guid guid)
        {
            var permCheck = new MultiPermissionsApp(BlockBuilder, appId, Log);
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
                throw exception;

            var appData = permCheck.App.Data;
            var item = appData.List.One(guid);
            var relationships = item.Relationships.AllRelationships;
            // var result = relationships.Select(r => new EntityInRelationDto(r.))
            // todo: don't forget Metadata relationships
            return null;
        }

        #endregion


        /// <summary>
        /// Get all Entities of specified Type
        /// </summary>
        [HttpGet]
	    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
	    public IEnumerable<Dictionary<string, object>> GetEntities(string contentType, int appId,
	        string cultureCode = null)
	        => new EntityApi(appId, true, Log).GetEntities(contentType, cultureCode);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<Dictionary<string, object>> GetAllOfTypeForAdmin(int appId, string contentType)
	    {
	        var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, contentType, Log);
	        if (!permCheck.EnsureAll(GrantSets.ReadSomething, out var exception))
	            throw exception;

            return new EntityApi(appId, true, Log).GetEntitiesForAdmin(contentType);
	    }


        [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, int id, int appId, bool force = false)
        {
            var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.DeleteSomething, out var exception))
                throw exception;
            new EntityApi(appId, true, Log).Delete(contentType, id, force);
        }

	    [HttpDelete]
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Delete(string contentType, Guid guid, int appId, bool force = false)
        {
            var permCheck = new MultiPermissionsTypes(BlockBuilder, appId, contentType, Log);
            if (!permCheck.EnsureAll(GrantSets.DeleteSomething, out var exception))
                throw exception;
            new EntityApi(appId, true, Log).Delete(contentType, guid, force);
        }





        #region Content Types
        /// <summary>
		/// Get a ContentType by Name
		/// </summary>
		[HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        public ContentTypeInfo GetContentType(string contentType, int appId) 
            => new Eav.WebApi.ContentTypeController().GetSingle(appId, contentType);

	    #endregion

        #region versioning

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<ItemHistory> History(int appId, [FromBody]ItemIdentifier item)
	    {
            ResolveItemIdOfGroup(appId, item);
	        return new AppManager(appId, Log).Entities.VersionHistory(item.EntityId);
        }

	    private void ResolveItemIdOfGroup(int appId, ItemIdentifier item)
	    {
            if (item.Group == null) return;
            var cms = new CmsRuntime(appId, Log, true);

            var contentGroup = cms.Blocks.GetBlockConfig(item.Group.Guid);
	        var part = contentGroup[item.Group.Part];
	        item.EntityId = part[item.ListIndex()].EntityId;
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
