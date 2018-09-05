using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Persistence;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)] // while in dev-mode, only for super-users
    public class UiController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.UiCont");
        }


        [HttpPost]
        public AllInOne Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            // Security check
            Log.Add($"load many a#{appId}, items⋮{items.Count}");

            // to do full security check, we'll have to see what content-type is requested
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, items, out var exp))
                throw exp;
            permCheck.InitAppData();

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            var typeRead = entityApi.AppManager.Read.ContentTypes;
            items = EntitiesController.ConvertListIndexToId(items, permCheck.App);
            var list = entityApi.GetEntitiesForEditing(appId, items);
            result.Items = list.Select(e => new HeaderAndJsonEntity
            {
                Header = e.Header,
                Entity = JsonSerializer.ToJson(e.Entity ?? ConstructEmptyEntity(appId, e.Header, typeRead))
            }).ToList();

            // set published if some data already exists
            if(list.Any())
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true

            // load content-types
            var types = 
                list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();
            result.ContentTypes = types.Select(JsonSerializer.ToJson).ToList();

            // load input-field configurations
            var fields = types
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputTypeTempBetterForNewUi)
                .Distinct();
            result.InputTypes = typeRead.GetInputTypes()
                .Where(it => fields.Contains(it.Type))
                .ToList();

            // also deliver features
            result.Features = SystemController.FeatureListWithPermissionCheck(appId, permCheck);

            // done - return
            return result;
        }

        private static IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead)
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = EntityBuilder.EntityWithAttributes(appId, header.Guid, header.EntityId, 0, type);
            return ent;
        }

        [HttpPost]
        public string Save([FromBody] AllInOne package, int appId, bool partOfPage)
        {
            var validator = new SaveDataValidator(package, Log);
            // perform some basic validation checks
            if (!validator.ContainsOnlyExpectedNodes(out var exp))
                throw exp;

            // todo: check contentblockappid in group-header, because this is where it should be saved!
            var contextAppId = appId;
            var targetAppId = package.Items.First().Header.Group.ContentBlockAppId;
            if (targetAppId != 0)
            {
                Log.Add($"detected content-block app to use: {targetAppId}; in context of app {contextAppId}");
                appId = targetAppId;
            }

            var appMan = new AppManager(appId, Log);
            var appRead = appMan.Read;
            var ser = new JsonSerializer(appRead.Package, Log);
            validator.PrepareForEntityChecks(appRead);

            #region permission checks
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, package.Items.Select(i => i.Header).ToList(), out exp))
                throw exp;
            if (!permCheck.UserUnrestrictedAndFeatureEnabled(out exp))
                throw exp;
            #endregion

            var count = 0;
            var itemsToSave = package.Items.Select(i => new HeaderJsonAndEntity(i));
            foreach (var item in itemsToSave)
            {
                item.RealEntity = ser.Deserialize(item.Entity, false, false);
                if (!validator.EntityIsOk(count, item.RealEntity, out exp))
                    throw exp;


                // todo: check how to handle (save/not-save) presentation and other virtual items
                // - seems to use .Group.SlotIsEmpty and .Group.Add as reference
                Log.Add("all data tests passed");

                // Temp solution - this should be a real entity
                if (!item.Header.Group.SlotIsEmpty)
                {
                    Log.Add("Will save...");
                    var saveOptions = SaveOptions.Build(appMan.ZoneId);
                    saveOptions.DiscardattributesNotInType = true;
                    saveOptions.DraftShouldBranch = package.DraftShouldBranch;
                    saveOptions.PreserveUntouchedAttributes = true;
                    saveOptions.PreserveExistingLanguages = true;

                    // can't save yet - too many pieces missing
                    //appMan.Entities.Save(item.RealEntity, saveOptions);

                }
                else // slot is empty, may need to remove
                {
                    Log.Add("will not save - maybe remove item if it already existed (slot should be empty");
                    // todo: handle this
                }

                count++;
            }

            // todo: update group if this was added / changed

            return "call to save worked - save doesn't work yet";
        }
        
    }
}
