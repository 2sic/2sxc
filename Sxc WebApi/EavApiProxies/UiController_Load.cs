using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    public partial class UiController
    {

        [HttpPost]
        public AllInOne Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            // Security check
            var wraplog = Log.Call("Load", $"load many a#{appId}, items⋮{items.Count}");

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var appForSecurityChecks = App.LightWithoutData(new DnnTenant(PortalSettings), appId, Log);
            items = new SaveHelpers.ContentGroupList(SxcInstance, Log).ConvertListIndexToId(items, appForSecurityChecks);

            // now look up the types, and repeat security check with type-names
            var permCheck = new MultiPermissionsTypes(SxcInstance, appId, items, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exception))
                throw exception;

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            var typeRead = entityApi.AppManager.Read.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(appId, items);
            var jsonSerializer = new JsonSerializer();
            result.Items = list.Select(e =>
            {
                // attach original metadata assignment when creating a new one
                JsonEntity ent;
                if (e.Entity != null)
                    ent = jsonSerializer.ToJson(e.Entity);
                else
                {
                    ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, e.Header, typeRead));
                    if (ent.For == null && e.Header?.For != null)
                        ent.For = e.Header.For;
                }

                return new BundleWithHeader<JsonEntity>
                {
                    Header = e.Header,
                    Entity = ent
                };
            }).ToList();

            // set published if some data already exists
            if(list.Any())
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true

            // load content-types
            var types = UsedTypes(list, typeRead);
            result.ContentTypes = types.Select(ct => JsonSerializer.ToJson(ct, true)).ToList();

            // load input-field configurations
            result.InputTypes = GetNecessaryInputTypes(types, typeRead);

            // also deliver features
            result.Features = SystemController.FeatureListWithPermissionCheck(appId, permCheck).ToList();

            // done - return
            wraplog($"ready, sending items:{result.Items.Count}, " +
                    $"types:{result.ContentTypes.Count}, " +
                    $"inputs:{result.InputTypes.Count}, " +
                    $"feats:{result.Features.Count}");
            return result;
        }

        private static List<IContentType> UsedTypes(List<BundleIEntity> list, ContentTypeRuntime typeRead)
            => list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();

        private static List<InputTypeInfo> GetNecessaryInputTypes(List<IContentType> types, ContentTypeRuntime typeRead)
        {
            var fields = types
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputTypeTempBetterForNewUi)
                .Distinct();
            return typeRead.GetInputTypes()
                .Where(it => fields.Contains(it.Type))
                .ToList();
        }

        private static IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead)
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = EntityBuilder.EntityWithAttributes(appId, header.Guid, header.EntityId, 0, type);
            return ent;
        }

    }
}
