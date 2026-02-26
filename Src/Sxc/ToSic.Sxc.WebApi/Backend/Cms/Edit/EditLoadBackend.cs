using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.WebApi.Sys.Entities;
using ToSic.Sxc.Backend.Cms.Load.Activities;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sys.Security.Permissions;


namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditLoadBackend(
    AppWorkContextService workCtxSvc,
    EditLoadActionGetForEditing actGetForEditing,
    ISxcCurrentContextService ctxService,
    Generator<MultiPermissionsTypes, MultiPermissionsTypes.Options> typesPermissions,
    LazySvc<DataValidatorContentTypeDataStore> valContentTypeDataStore,

    EditLoadActivityCleanupRequest actCleanupRequest,
    EditLoadActivityConvertRequest actConvertRequest,
    EditLoadActivityAddContentTypes actAddContentTypes,
    EditLoadActivityAddNecessaryInputTypes actAddNecessaryInputTypes,
    EditLoadActivityAddContext actAddContext,
    EditLoadActivityAddRequiredFeatures actAddRequiredFeatures,
    EditLoadActivityAddPrefetch actAddPrefetch,
    EditLoadActivitySettingsHelper actAddActivitySettings
)
    : ServiceBase("Cms.LoadBk",
        connect:
        [
            workCtxSvc, actGetForEditing, ctxService, typesPermissions, valContentTypeDataStore, actAddPrefetch, actAddActivitySettings,
            actCleanupRequest,
            actConvertRequest,
            actAddContentTypes,
            actAddNecessaryInputTypes,
            actAddContext,
            actAddRequiredFeatures,
        ])
{
    public async Task<EditLoadDto> Load(int appId, List<ItemIdentifier> items)
    {
        var l = Log.Fn<EditLoadDto>($"load many a#{appId}, items⋮{items.Count}");

        var appContext = ctxService.GetExistingAppOrSet(appId);

        // Note 2026-02-26 2dm - changed this to use from context, should be identical, but maybe it's not? keep an eye on this till 2026-Q2
        var appReader = appContext.AppReaderRequired; // appReaders.Get(appId);
        var actContext = new LowCodeActionContext
        {
            Context = new(StringComparer.OrdinalIgnoreCase)
            {
                [EditLoadContextConstants.AppId] = appId,
                [EditLoadContextConstants.AppReader] = appReader,
                [EditLoadContextConstants.AppContext] = appContext,
            }
        };

        var itemsData = await actCleanupRequest.Run(actContext, ActionData.Create(items));

        // Security check
        // do early permission check - but at this time it may be that we don't have the types yet
        // because they may be group/id combinations, without type information which we'll look up afterward

        // Special Edge Case
        // If the user is Module-Admin then we can skip the remaining checks
        // This is important because the main context may not contain the module

        // Look up the types, and repeat security check with type-names
        l.A($"Will do permission check; app has {appReader.List.Count} items");
        var usedContentTypes = new SavePermissionDataHelper(Log).ExtractTypeNamesFromItems(appReader, itemsData.Data);
        var permCheck = typesPermissions.New(new()
        {
            SiteContext = appContext,
            App = appReader,
            ContentTypes = usedContentTypes,
        });
        if (!permCheck.EnsureAll(GrantSets.WriteSomething, out var error))
            throw HttpException.PermissionDenied(error);

        // load items - similar
        var showDrafts = permCheck.EnsureAny(GrantSets.ReadDraft);
        var appWorkCtx = workCtxSvc.ContextPlus(appId, showDrafts: showDrafts);

        actContext = actContext.With(EditLoadContextConstants.AppCtxWork, appWorkCtx);

        var list = actGetForEditing.Run(actContext, itemsData.Data);

        // Do special PreLoad checks
        for (var index = 0; index < list.Count; index++)
        {
            var ent = list[index].Entity;
            if (ent == null)
                continue; // new item, so no need to check
            var preEdit = await valContentTypeDataStore.Value.PreEdit(index, ent);
            if (preEdit.Exception != null)
                throw preEdit.Exception;
        }

        var result = await actConvertRequest.Run(actContext, ActionData.Create(list));

        // since we're retrieving data - make sure we're allowed to
        // this is to ensure that if public forms only have "create" permissions, they can't access existing data
        // important, this code is shared/duplicated in the EntitiesController.GetManyForEditing
        if (list.Any(set => set.Entity != null))
            if (!permCheck.EnsureAll(GrantSets.ReadSomething, out error))
                throw l.Ex(HttpException.PermissionDenied(error));


        var usedTypes = UsedTypes(list, appReader);
        actContext = actContext.With(EditLoadContextConstants.UsedTypes, usedTypes);

        var actions = new List<ILowCodeAction<EditLoadDto, EditLoadDto>>
        {
            actAddContentTypes,         // Add Content Types information
            actAddNecessaryInputTypes,  // load input-field configurations
            actAddContext,              // Attach context, but only the minimum needed for the UI
            actAddActivitySettings,     // Load settings for the front-end
            actAddPrefetch,             // Prefetch additional data
            actAddRequiredFeatures      // Determine required features for the UI
        };

        // Loop through the actions and run each one
        foreach (var lowCodeAction in actions)
            result = await lowCodeAction.Run(actContext, result);

        // done
        var final = result.Data;
        var finalMsg = $"items:{final.Items.Count}, types:{final.ContentTypes.Count}, inputs:{final.InputTypes.Count}, feats:{final.Context.Features?.Count}";
        return l.Return(final, finalMsg);
    }
        


    private List<IContentType> UsedTypes(List<BundleWithHeaderOptional<IEntity>> list, IAppReader appReader)
        => list.Select(i
                // try to get the entity type, but if there is none (new), look it up according to the header
                => i.Entity?.Type
                   ?? appReader.GetContentType(i.Header!.ContentTypeName!))
            .ToList();

}