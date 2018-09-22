using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)] // while in dev-mode, only for super-users
    public partial class UiController
    {

        [HttpPost]
        public AllInOne Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            // Security check
            Log.Add($"load many a#{appId}, items⋮{items.Count}");

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var permCheck = new MultiPermissionsApp(SxcInstance, appId, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;
            permCheck.InitializeData();

            // now look up the types, and repeat security check with type-names
            items = new SaveHelpers.ContentGroupList(SxcInstance, Log).ConvertListIndexToId(items, permCheck.App);
            permCheck = new MultiPermissionsTypes(SxcInstance, appId, items, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out exp))
                throw exp;

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            var typeRead = entityApi.AppManager.Read.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(appId, items);
            result.Items = list.Select(e => new BundleWithHeader<JsonEntity>
            {
                Header = e.Header,
                Entity = JsonSerializer.ToJson(e.Entity ?? ConstructEmptyEntity(appId, e.Header, typeRead))
            }).ToList();

            // set published if some data already exists
            if(list.Any())
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true

            // load content-types
            var types = UsedTypes(list, typeRead);
            result.ContentTypes = types.Select(JsonSerializer.ToJson).ToList();

            // load input-field configurations
            result.InputTypes = GetNecessaryInputTypes(types, typeRead);

            // also deliver features
            result.Features = SystemController.FeatureListWithPermissionCheck(appId, permCheck);

            // done - return
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
