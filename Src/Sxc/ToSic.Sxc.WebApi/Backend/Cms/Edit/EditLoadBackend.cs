using ToSic.Eav;
using ToSic.Eav.Data.Build;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Internal;
using ToSic.Eav.Serialization.Internal;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Backend.SaveHelpers;
using JsonSerializer = ToSic.Eav.ImportExport.Json.JsonSerializer;


namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class EditLoadBackend(
    AppWorkContextService workCtxSvc,
    EntityApi api,
    ContentGroupList contentGroupList,
    EntityBuilder entityBuilder,
    IUiContextBuilder contextBuilder,
    ISxcContextResolver ctxResolver,
    ITargetTypes mdTargetTypes,
    IAppReaderFactory appReaders,
    IUiData uiData,
    GenWorkPlus<WorkInputTypes> inputTypes,
    Generator<JsonSerializer> jsonSerializerGenerator,
    Generator<MultiPermissionsTypes> typesPermissions,
    EditLoadPrefetchHelper prefetch,
    EditLoadSettingsHelper loadSettings)
    : ServiceBase("Cms.LoadBk",
        connect:
        [
            workCtxSvc, inputTypes, api, contentGroupList, entityBuilder, contextBuilder, ctxResolver,
            mdTargetTypes, appReaders, uiData, jsonSerializerGenerator, typesPermissions, prefetch, loadSettings
        ])
{


    public EditDto Load(int appId, List<ItemIdentifier> items)
    {
        var l = Log.Fn<EditDto>($"load many a#{appId}, items⋮{items.Count}");
        // Security check
        var context = ctxResolver.GetBlockOrSetApp(appId);
        
        // do early permission check - but at this time it may be that we don't have the types yet
        // because they may be group/id combinations, without type information which we'll look up afterward
        var appReader = appReaders.Get(appId);
        items = contentGroupList.Init(appReader.PureIdentity())
            .ConvertGroup(items)
            .ConvertListIndexToId(items);
        TryToAutoFindMetadataSingleton(items, context.AppReader.Metadata);

        // Special Edge Case
        // If the user is Module-Admin then we can skip the remaining checks
        // This is important because the main context may not contain the module

        // Look up the types, and repeat security check with type-names
        var permCheck = typesPermissions.New().Init(context, context.AppReader, items);
        if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
            throw HttpException.PermissionDenied(error);

        // load items - similar
        var showDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);
        var appWorkCtx = workCtxSvc.ContextPlus(appId, showDrafts: showDrafts);
        var result = new EditDto();
        var entityApi = api.Init(appId, showDrafts);
        var list = entityApi.GetEntitiesForEditing(items);
        var jsonSerializer = jsonSerializerGenerator.New().SetApp(appReader);
        result.Items = list
            .Select(e => new BundleWithHeader<JsonEntity>
            {
                Header = e.Header,
                Entity = GetSerializeAndMdAssignJsonEntity(appId, e, jsonSerializer, appReader, appWorkCtx)
            })
            .ToList();

        // set published if some data already exists
        if (list.Any())
        {
            var entity = list.First().Entity;
            result.IsPublished = entity?.IsPublished ?? true; // Entity could be null (new), then true
            // only set draft-should-branch if this draft already has a published item
            if (!result.IsPublished)
                result.DraftShouldBranch = (entity == null ? null : appReader.GetPublished(entity)) != null;
        }

        // since we're retrieving data - make sure we're allowed to
        // this is to ensure that if public forms only have create permissions, they can't access existing data
        // important, this code is shared/duplicated in the EntitiesController.GetManyForEditing
        if (list.Any(set => set.Entity != null))
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out error))
                throw l.Ex(HttpException.PermissionDenied(error));

        #region Load content-types and additional data (like formulas)

        var serializerForTypes = jsonSerializerGenerator.New().SetApp(appReader);
        serializerForTypes.ValueConvertHyperlinks = true;
        var usedTypes = UsedTypes(list, appWorkCtx);
        var serSettings = new JsonSerializationSettings
        {
            CtIncludeInherited = true,
            CtAttributeIncludeInheritedMetadata = true
        };

        var jsonTypes = usedTypes
            .Select(t => serializerForTypes.ToPackage(t, serSettings))
            .ToList();

        result.ContentTypes = jsonTypes
            .Select(t => t.ContentType)
            .ToList();

        // Also add global Entities like Formulas which would not be included otherwise
        result.ContentTypeItems = jsonTypes
            .SelectMany(t => t.Entities)
            .ToList();

        var isSystemType = usedTypes.Any(t => t.AppId == Constants.PresetAppId);
        l.A($"isSystemType: {isSystemType}");

        #endregion

        #region Input Types on ContentTypes and general definitions

        // Fix not-supported input-type names; map to correct name
        result.ContentTypes
            .ForEach(jt => jt.Attributes
                .ForEach(at => at.InputType = Compatibility.Internal.InputTypes.MapInputTypeV10(at.InputType)));

        // load input-field configurations
        result.InputTypes = GetNecessaryInputTypes(result.ContentTypes, appWorkCtx);

        #endregion

        // Attach context, but only the minimum needed for the UI
        result.Context = contextBuilder.InitApp(context.AppReader)
            .Get(Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.Features | (isSystemType ? Ctx.FeaturesForSystemTypes : Ctx.Features), CtxEnable.EditUi);

        // Load settings for the front-end
        result.Settings = loadSettings.GetSettings(context, usedTypes, result.ContentTypes, appWorkCtx);

        // Prefetch additional data
        try
        {
            result.Prefetch = prefetch.TryToPrefectAdditionalData(appId, result);
        }
        catch (Exception ex) // Log and Ignore
        {
            l.A("Ran into an error during Prefetch");
            l.Ex(ex);
        }

        // Determine required features for the UI WIP 18.02
        var inheritedFields = usedTypes
            .SelectMany(t => t.Attributes
                .Where(a => a.SysSettings?.InheritMetadata == true)
                .Select(a => new { Name = a.Name, Type = t}))
            .ToList();

        if (inheritedFields.Any())
            result.RequiredFeatures = new()
            {
                { BuiltInFeatures.ContentTypeFieldsReuseDefinitions.NameId, inheritedFields.Select(f => $"Used in fields: {f.Type.Name}.{f.Name}").ToArray() },
            };

        // done
        var finalMsg = $"items:{result.Items.Count}, types:{result.ContentTypes.Count}, inputs:{result.InputTypes.Count}, feats:{result.Context.Features.Count}";
        return l.Return(result, finalMsg);
    }
        

    /// <summary>
    /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
    /// </summary>
    private bool TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, IMetadataSource appMdSource)
    {
        var l = Log.Fn<bool>();
        foreach (var header in list
                     .Where(header => header.For?.Singleton == true && header.ContentTypeName.HasValue()))
        {
            l.A("Found an entity with the auto-lookup marker");
            // try to find metadata for this
            var mdFor = header.For;
            // #TargetTypeIdInsteadOfTarget
            var type = mdFor.TargetType != 0 ? mdFor.TargetType : mdTargetTypes.GetId(mdFor.Target);
            var mds = mdFor.Guid != null
                ? appMdSource.GetMetadata(type, mdFor.Guid.Value, header.ContentTypeName)
                : mdFor.Number != null
                    ? appMdSource.GetMetadata(type, mdFor.Number.Value, header.ContentTypeName)
                    : appMdSource.GetMetadata(type, mdFor.String, header.ContentTypeName);

            var mdList = mds.ToArray();
            if (mdList.Length > 1)
            {
                l.A($"Warning - looking for best metadata but found too many {mdList.Length}, will use first");
                // must now sort by ID otherwise the order may be different after a few save operations
                mdList = [.. mdList.OrderBy(e => e.EntityId)];
            }
            header.EntityId = !mdList.Any() ? 0 : mdList.First().EntityId;
        }

        return l.ReturnTrue();
    }
}