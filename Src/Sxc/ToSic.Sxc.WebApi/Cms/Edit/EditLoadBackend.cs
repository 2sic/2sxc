using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.DI;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services.GoogleMaps;
using ToSic.Sxc.WebApi.Save;
using JsonSerializer = ToSic.Eav.ImportExport.Json.JsonSerializer;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend: WebApiBackendBase<EditLoadBackend>
    {

        #region DI Constructor

        public EditLoadBackend(EntityApi entityApi,
            ContentGroupList contentGroupList,
            EntityBuilder entityBuilder,
            IServiceProvider serviceProvider,
            IUiContextBuilder contextBuilder,
            IContextResolver ctxResolver,
            ITargetTypes mdTargetTypes,
            EntityPickerApi entityPickerBackend,
            IAppStates appStates,
            IUiData uiData,
            Generator<JsonSerializer> jsonSerializerGenerator,
            GoogleMapsSettings googleMapsSettings,
            LazyInitLog<AppSettingsStack> settingsStack
        ) : base(serviceProvider, "Cms.LoadBk") =>
            ConnectServices(
                _entityApi = entityApi,
                _contentGroupList = contentGroupList,
                _entityBuilder = entityBuilder,
                _contextBuilder = contextBuilder,
                _ctxResolver = ctxResolver,
                _mdTargetTypes = mdTargetTypes,
                _entityPickerBackend = entityPickerBackend,
                _appStates = appStates,
                _uiData = uiData,
                _jsonSerializerGenerator = jsonSerializerGenerator,
                _settingsStack = settingsStack,
                _googleMapsSettings = googleMapsSettings
            );

        private readonly EntityApi _entityApi;
        private readonly ContentGroupList _contentGroupList;
        private readonly EntityBuilder _entityBuilder;
        private readonly IUiContextBuilder _contextBuilder;
        private readonly IContextResolver _ctxResolver;
        private readonly ITargetTypes _mdTargetTypes;
        private readonly EntityPickerApi _entityPickerBackend;
        private readonly IAppStates _appStates;
        private readonly IUiData _uiData;
        private readonly Generator<JsonSerializer> _jsonSerializerGenerator;
        private readonly LazyInitLog<AppSettingsStack> _settingsStack;
        private readonly GoogleMapsSettings _googleMapsSettings;

        #endregion


        public EditDto Load(int appId, List<ItemIdentifier> items)
        {
            // Security check
            var l = Log.Fn<EditDto>($"load many a#{appId}, items⋮{items.Count}");

            var context = _ctxResolver.BlockOrApp(appId);

            var showDrafts = context.UserMayEdit;

            // do early permission check - but at this time it may be that we don't have the types yet
            // because they may be group/id combinations, without type information which we'll look up afterwards
            var appIdentity = _appStates.IdentityOfApp(appId);
            items = _contentGroupList.Init(appIdentity, Log, showDrafts)
                .ConvertGroup(items)
                .ConvertListIndexToId(items);
            TryToAutoFindMetadataSingleton(items, context.AppState);

            // now look up the types, and repeat security check with type-names
            var permCheck = GetService<MultiPermissionsTypes>().Init(context, context.AppState, items, Log);
            if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
                throw HttpException.PermissionDenied(error);

            // load items - similar
            var result = new EditDto();
            var entityApi = _entityApi.Init(appId, permCheck.EnsureAny(GrantSets.ReadDraft), Log);
            var typeRead = entityApi.AppRead.ContentTypes;
            var list = entityApi.GetEntitiesForEditing(items);
            var jsonSerializer = _jsonSerializerGenerator.New().Init(entityApi.AppRead.AppState, Log);
            result.Items = list.Select(e => new BundleWithHeader<JsonEntity>
            {
                Header = e.Header,
                Entity = GetSerializeAndMdAssignJsonEntity(appId, e, jsonSerializer, typeRead, _appStates.Get(appIdentity))
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
            var serializerForTypes = _jsonSerializerGenerator.New().Init(entityApi.AppRead.AppState, Log);
            serializerForTypes.ValueConvertHyperlinks = true;
            var types = UsedTypes(list, typeRead);
            var jsonTypes = types.Select(t => serializerForTypes.ToPackage(t, true)).ToList();
            result.ContentTypes = jsonTypes.Select(t => t.ContentType).ToList();

            result.ContentTypeItems = jsonTypes.SelectMany(t => t.Entities).ToList();

            // Fix not-supported input-type names; map to correct name
            result.ContentTypes
                .ForEach(jt => jt.Attributes
                    .ForEach(at => at.InputType = Compatibility.InputTypes.MapInputTypeV10(at.InputType)));

            // load input-field configurations
            result.InputTypes = GetNecessaryInputTypes(result.ContentTypes, typeRead);

            // also include UI features
            result.Features = _uiData.Features(permCheck);

            // Attach context, but only the minimum needed for the UI
            result.Context = _contextBuilder.InitApp(context.AppState, Log)
                .Get(Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.Features | Ctx.ApiKeys,
                    CtxEnable.EditUi);

            result.Settings = GetSettings(context);

            try
            {
                result.Prefetch = TryToPrefectAdditionalData(appId, result);
            }
            catch (Exception ex)
            {
                Log.A("Ran into an error during Prefetch");
                Log.Ex(ex);
                /* ignore */
            }



            // done
            return l.Return(result, $"ready, sending items:{result.Items.Count}, " +
                                   $"types:{result.ContentTypes.Count}, " +
                                   $"inputs:{result.InputTypes.Count}, " +
                                   $"feats:{result.Features.Count}");
        }
        

        /// <summary>
        /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
        /// </summary>
        private bool TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, AppState appState)
        {
            var wrapLog = Log.Fn<bool>();

            foreach (var header in list
                .Where(header => header.For?.Singleton == true && !string.IsNullOrWhiteSpace(header.ContentTypeName)))
            {
                Log.A("Found an entity with the auto-lookup marker");
                // try to find metadata for this
                var mdFor = header.For;
                // #TargetTypeIdInsteadOfTarget
                var type = mdFor.TargetType != 0 ? mdFor.TargetType : _mdTargetTypes.GetId(mdFor.Target);
                var mds = mdFor.Guid != null
                    ? appState.GetMetadata(type, mdFor.Guid.Value, header.ContentTypeName)
                    : mdFor.Number != null
                        ? appState.GetMetadata(type, mdFor.Number.Value, header.ContentTypeName)
                        : appState.GetMetadata(type, mdFor.String, header.ContentTypeName);

                var mdList = mds.ToArray();
                if (mdList.Length > 1)
                {
                    Log.A($"Warning - looking for best metadata but found too many {mdList.Length}, will use first");
                    // must now sort by ID otherwise the order may be different after a few save operations
                    mdList = mdList.OrderBy(e => e.EntityId).ToArray();
                }
                header.EntityId = !mdList.Any() ? 0 : mdList.First().EntityId;
            }

            return wrapLog.ReturnTrue("ok");
        }
    }
}

