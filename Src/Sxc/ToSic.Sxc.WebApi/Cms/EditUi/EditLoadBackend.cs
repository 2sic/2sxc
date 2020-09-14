using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Features;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    internal partial class EditLoadBackend: WebApiBackendBase<EditLoadBackend>
    {
        public EditLoadBackend() : base("Cms.LoadBk")
        {
        }

        public AllInOneDto Load(IBlock block, IContextBuilder contextBuilder, int appId, List<ItemIdentifier> items)
        {
            // Security check
            var wraplog = Log.Call($"load many a#{appId}, items⋮{items.Count}");

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var appIdentity = State.Identity(null, appId);
            //var block = GetBlock();
            items = new ContentGroupList(block, Log).ConvertListIndexToId(items, appIdentity);

            // now look up the types, and repeat security check with type-names
            // todo: 2020-03-20 new feat 11.01, may not check inner type permissions ATM
            var permCheck = new MultiPermissionsTypes().Init(block.Context, GetApp(appId, block), items, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // load items - similar
            var result = new AllInOneDto();
            var entityApi = new EntityApi(appId, permCheck.EnsureAny(GrantSets.ReadDraft), Log);
            var typeRead = entityApi.AppRead.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(items);
            var jsonSerializer = new JsonSerializer();
            result.Items = list.Select(e => new BundleWithHeader<JsonEntity>
            {
                Header = e.Header,
                Entity = GetSerializeAndMdAssignJsonEntity(appId, e, jsonSerializer, typeRead)
            }).ToList();

            // set published if some data already exists
            if (list.Any())
            {
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true
                // only set draft-should-branch if this draft already has a published item
                if (!result.IsPublished) result.DraftShouldBranch = list.First().Entity?.GetPublished() != null;
            }

            // since we're retrieving data - make sure we're allowed to
            // this is to ensure that if public forms only have create permissions, they can't access existing data
            // important, this code is shared/duplicated in the EntitiesController.GetManyForEditing
            if (list.Any(set => set.Entity != null))
                if (!permCheck.EnsureAll(GrantSets.ReadSomething, out error))
                    throw HttpException.PermissionDenied(error);

            // load content-types
            var types = UsedTypes(list, typeRead);
            result.ContentTypes = types
                .Select(ct => JsonSerializer.ToJson(ct, true))
                .ToList();

            // todo: ensure that sub-properties of the content-types are included
            var entList = types.SelectMany(
                // in all Content-Type attributes like title, body etc.
                t => t.Attributes.SelectMany(
                    // check all metadata of these attributes - get possible sub-entities attached
                    a => a.Metadata.SelectMany(m => m.Children())
                )
            );
            result.ContentTypeItems = entList.Select(e => jsonSerializer.ToJson(e, 0, Log)).ToList();

            // Fix not-supported input-type names; map to correct name
            result.ContentTypes
                .ForEach(jt => jt.Attributes
                    .ForEach(at => at.InputType = InputTypes.MapInputTypeV10(at.InputType)));

            // load input-field configurations
            result.InputTypes = GetNecessaryInputTypes(result.ContentTypes, typeRead);

            // also include UI features
            result.Features = FeaturesHelpers.FeatureListWithPermissionCheck(permCheck).ToList();

            // Attach context
            result.Context = contextBuilder.InitApp(appIdentity.ZoneId, permCheck.App)
                .Get(Ctx.AppBasic | Ctx.Language | Ctx.Site | Ctx.System);

            // done - return
            wraplog($"ready, sending items:{result.Items.Count}, " +
                    $"types:{result.ContentTypes.Count}, " +
                    $"inputs:{result.InputTypes.Count}, " +
                    $"feats:{result.Features.Count}");
            return result;
        }
    }
}

