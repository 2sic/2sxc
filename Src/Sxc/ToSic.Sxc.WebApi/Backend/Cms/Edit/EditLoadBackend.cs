using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Data.Build;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Metadata.Sys;
using ToSic.Eav.Metadata.Targets;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.WebApi.Security;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sxc.Data.Sys;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Utils;
using JsonSerializer = ToSic.Eav.ImportExport.Json.Sys.JsonSerializer;


namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class EditLoadBackend(
    AppWorkContextService workCtxSvc,
    EntityApi api,
    ContentGroupList contentGroupList,
    EntityBuilder entityBuilder,
    IUiContextBuilder contextBuilder,
    ISxcCurrentContextService ctxService,
    ITargetTypeService mdTargetTypes,
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
            workCtxSvc, inputTypes, api, contentGroupList, entityBuilder, contextBuilder, ctxService,
            mdTargetTypes, appReaders, uiData, jsonSerializerGenerator, typesPermissions, prefetch, loadSettings
        ])
{


    public EditLoadDto Load(int appId, List<ItemIdentifier> items)
    {
        var l = Log.Fn<EditLoadDto>($"load many a#{appId}, items⋮{items.Count}");
        // Security check
        var context = ctxService.GetExistingAppOrSet(appId);
        
        // do early permission check - but at this time it may be that we don't have the types yet
        // because they may be group/id combinations, without type information which we'll look up afterward
        var appReader = appReaders.Get(appId);
        items = contentGroupList.Init(appReader.PureIdentity())
            .ConvertGroup(items)
            .ConvertListIndexToId(items);
        TryToAutoFindMetadataSingleton(items, context.AppReaderRequired.Metadata);

        // Special Edge Case
        // If the user is Module-Admin then we can skip the remaining checks
        // This is important because the main context may not contain the module

        // Look up the types, and repeat security check with type-names
        l.A($"Will do permission check; app has {context.AppReaderRequired.List.Count} items");
        var permCheck = typesPermissions.New()
            .Init(context, context.AppReaderRequired, items);
        if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
            throw HttpException.PermissionDenied(error);

        // load items - similar
        var showDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);
        var appWorkCtx = workCtxSvc.ContextPlus(appId, showDrafts: showDrafts);
        var entityApi = api.Init(appId, showDrafts);
        var list = entityApi.GetEntitiesForEditing(items);
        var jsonSerializer = jsonSerializerGenerator.New().SetApp(appReader);

        var result = new EditLoadDto
        {
            Items = list
                .Select(e => new BundleWithHeaderOptional<JsonEntity>
                {
                    Header = e.Header,
                    Entity = GetSerializeAndMdAssignJsonEntity(appId, e, jsonSerializer, appReader, appWorkCtx)
                })
                .ToList(),
        };

        // set published if some data already exists
        if (list.Any())
        {
            var entity = list.First().Entity;
            var isPublished = entity?.IsPublished ?? true; // Entity could be null (new), then true
            result = result with
            {
                IsPublished = isPublished,
                // only set draft-should-branch if this draft already has a published item
                DraftShouldBranch = !isPublished && (appReader.GetPublished(entity)) != null
            };
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
        var isSystemType = usedTypes.Any(t => t.AppId == KnownAppsConstants.PresetAppId);
        l.A($"isSystemType: {isSystemType}");



        var serSettings = new JsonSerializationSettings
        {
            CtIncludeInherited = true,
            CtAttributeIncludeInheritedMetadata = true
        };

        var jsonTypes = usedTypes
            .Select(t => serializerForTypes.ToPackage(t, serSettings))
            .ToListOpt();

        // Fix not-supported input-type names; map to correct name
        jsonTypes = jsonTypes
            .Select(jt =>
            {
                jt = jt with
                {
                    ContentType = jt.ContentType == null
                        ? null
                        : jt.ContentType with
                        {
                            Attributes = (jt.ContentType.Attributes ?? [])
                            .Select(a => a with
                            {
                                // ensure that the input-type is set, otherwise it will be null
                                InputType = InputTypes.MapInputTypeV10(a.InputType! /* it can't really be null, only in very old imports, and this is not an import */)
                            })
                            .ToListOpt(),
                        }
                };
                return jt;
            })
            .ToListOpt();

        // Old, non functional
        // Fix not-supported input-type names; map to correct name
        //foreach (var at in result.ContentTypes.SelectMany(jt => jt.AttributesSafe()))
        //    at.InputType = InputTypes.MapInputTypeV10(at.InputType);

        result = result with
        {
            ContentTypes = jsonTypes
                .Select(t => t.ContentType!)
                .ToList(),

            // Also add global Entities like Formulas which would not be included otherwise
            ContentTypeItems = jsonTypes
                .SelectMany(t => t.Entities!)
                .ToList(),
        };

        #endregion

        #region Input Types on ContentTypes and general definitions


        // load input-field configurations
        result = result with
        {
            InputTypes = GetNecessaryInputTypes(result.ContentTypes, appWorkCtx),
        };

        #endregion

        // Attach context, but only the minimum needed for the UI
        result = result with
        {
            Context = contextBuilder.InitApp(context.AppReaderRequired)
                .Get(
                    Ctx.AppBasic | Ctx.AppEdit | Ctx.Language | Ctx.Site | Ctx.System | Ctx.User | Ctx.Features |
                    (isSystemType ? Ctx.FeaturesForSystemTypes : Ctx.Features), CtxEnable.EditUi),

            // Load settings for the front-end
            Settings = loadSettings.GetSettings(context, usedTypes, result.ContentTypes, appWorkCtx),
        };

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
                .Select(a => new { a.Name, Type = t}))
            .ToList();

        if (inheritedFields.Any())
            result = result with
            {
                RequiredFeatures = new()
                {
                    {
                        BuiltInFeatures.ContentTypeFieldsReuseDefinitions.NameId,
                        inheritedFields.Select(f => $"Used in fields: {f.Type.Name}.{f.Name}").ToArray()
                    },
                }
            };

        // done
        var finalMsg = $"items:{result.Items.Count}, types:{result.ContentTypes.Count}, inputs:{result.InputTypes.Count}, feats:{result.Context.Features?.Count}";
        return l.Return(result, finalMsg);
    }
        

    /// <summary>
    /// new 2020-12-08 - correct entity-id with lookup of existing if marked as singleton
    /// </summary>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private bool TryToAutoFindMetadataSingleton(List<ItemIdentifier> list, IMetadataSource appMdSource)
    {
        var l = Log.Fn<bool>();
        var headersWithMetadataFor = list
            .Where(header => header.For?.Singleton == true && header.ContentTypeName.HasValue())
            .ToListOpt();

        foreach (var header in headersWithMetadataFor)
        {
            l.A("Found an entity with the auto-lookup marker");
            // try to find metadata for this
            var mdFor = header.For;
            // #TargetTypeIdInsteadOfTarget
            var type = mdFor!.TargetType != 0
                ? mdFor.TargetType
                : mdTargetTypes.GetId(mdFor.Target!);
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