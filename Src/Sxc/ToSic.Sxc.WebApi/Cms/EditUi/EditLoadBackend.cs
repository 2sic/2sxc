using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Features;
using ToSic.Sxc.WebApi.Save;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend: WebApiBackendBase<EditLoadBackend>
    {
        //private const int TempToMarkSingletonMetadata = 2000000000;

        #region DI Constructor

        public EditLoadBackend(EntityApi entityApi, ContentGroupList contentGroupList, 
            IServiceProvider serviceProvider, 
            IUiContextBuilder contextBuilder, 
            IContextResolver ctxResolver, 
            ITargetTypes mdTargetTypes) : base(serviceProvider, "Cms.LoadBk")
        {
            _entityApi = entityApi;
            _contentGroupList = contentGroupList;
            _contextBuilder = contextBuilder;
            _ctxResolver = ctxResolver;
            _mdTargetTypes = mdTargetTypes;
        }
        
        private readonly EntityApi _entityApi;
        private readonly ContentGroupList _contentGroupList;
        private readonly IUiContextBuilder _contextBuilder;
        private readonly IContextResolver _ctxResolver;
        private readonly ITargetTypes _mdTargetTypes;

        #endregion


        public EditDto Load(int appId, List<ItemIdentifier> items)
        {
            // Security check
            var wrapLog = Log.Call($"load many a#{appId}, items⋮{items.Count}");

            var context = _ctxResolver.App(appId);

            var showDrafts = context.UserMayEdit;

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var appIdentity = State.Identity(null, appId);
            items = _contentGroupList.Init(appIdentity, Log, showDrafts).ConvertListIndexToId(items);
            TryToAutoFindMetadataSingleton(items, context.AppState);

            // now look up the types, and repeat security check with type-names
            // todo: 2020-03-20 new feat 11.01, may not check inner type permissions ATM
            var permCheck = ServiceProvider.Build<MultiPermissionsTypes>().Init(context, context.AppState, items, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // load items - similar
            var result = new EditDto();
            var entityApi = _entityApi.Init(appId, permCheck.EnsureAny(GrantSets.ReadDraft), Log);
            var typeRead = entityApi.AppRead.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(items);
            var jsonSerializer = ServiceProvider.Build<JsonSerializer>();
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

            // ensure that sub-properties of the content-types are included
            // this is for UI Formulas (children of @All) - WIP
            // and the warning/error Regex specials - WIP
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

            // Attach context, but only the minimum needed for the UI
            result.Context = _contextBuilder.SetZoneAndApp(appIdentity.ZoneId, context.AppState)
                .Get(Ctx.AppBasic | Ctx.Language | Ctx.Site | Ctx.System);

            // done - return
            wrapLog($"ready, sending items:{result.Items.Count}, " +
                    $"types:{result.ContentTypes.Count}, " +
                    $"inputs:{result.InputTypes.Count}, " +
                    $"feats:{result.Features.Count}");
            return result;
        }



        /// <summary>
        /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
        /// Singleton-marker is temporarily a very large number 2'000'000'000
        /// </summary>
        private bool TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, AppState appState)
        {
            var wrapLog = Log.Call<bool>();

            foreach (var header in list
                .Where(header => header.For?.Singleton == true && !string.IsNullOrWhiteSpace(header.ContentTypeName)))
            {
                Log.Add("Found an entity with the auto-lookup marker");
                // try to find metadata for this
                var mdFor = header.For;
                var type = _mdTargetTypes.GetId(mdFor.Target);
                var mds = mdFor.Guid != null
                    ? appState.GetMetadata(type, mdFor.Guid.Value, header.ContentTypeName)
                    : mdFor.Number != null
                        ? appState.GetMetadata(type, mdFor.Number.Value, header.ContentTypeName)
                        : appState.GetMetadata(type, mdFor.String, header.ContentTypeName);

                var mdList = mds.ToArray();
                if (mdList.Length > 1)
                {
                    Log.Add($"Warning - looking for best metadata but found too many {mdList.Length}, will use first");
                    // must now sort by ID otherwise the order may be different after a few save operations
                    mdList = mdList.OrderBy(e => e.EntityId).ToArray();
                }
                header.EntityId = !mdList.Any() ? 0 : mdList.First().EntityId;
            }

            return wrapLog("ok", true);
        }
    }
}

