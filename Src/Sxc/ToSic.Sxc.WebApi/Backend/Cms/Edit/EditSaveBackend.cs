using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditSaveBackend(
    SxcPagePublishing pagePublishing,
    GenWorkPlus<WorkEntities> workEntities,
    ISxcCurrentContextService ctxService,
    JsonSerializer jsonSerializer,
    SaveSecurity saveSecurity,
    SaveEntities saveBackendHelper,
    LazySvc<DataValidatorContentTypeDataStore> valContentTypeDataStore,
    DataBuilder dataBuilder)
    : ServiceBase("Cms.SaveBk", connect: [pagePublishing, workEntities, ctxService, jsonSerializer, saveSecurity, saveBackendHelper, dataBuilder, valContentTypeDataStore])
{
    public async Task<Dictionary<Guid, int>> Save(int appId, EditSaveDto package, bool partOfPage)
    {
        var l = Log.Fn<Dictionary<Guid, int>>($"save started with a#{appId}, i⋮{package.Items.Count}, partOfPage:{partOfPage}");

        // The context should be from the block if there is one, because it affects saving/publishing
        // Basically it can result in things being saved draft or titles being updated
        var context = ctxService.GetExistingAppOrSet(appId);


        // perform some basic validation checks
        var containsOnlyExpectedNodesException = new SaveDataPackageValidator(Log).ContainsOnlyExpectedNodes(package);
        if (containsOnlyExpectedNodesException != null)
            throw containsOnlyExpectedNodesException;

        // todo: unsure about this - thought I should check contentblockappid in group-header, because this is where it should be saved!
        //var contextAppId = appId;
        //var targetAppId = package.Items.First().Header.Group.ContentBlockAppId;
        //if (targetAppId != 0)
        //{
        //    Log.A($"detected content-block app to use: {targetAppId}; in context of app {contextAppId}");
        //    appId = targetAppId;
        //}

        // new API WIP
        var appEntities = workEntities.New(appId);
        var appCtx = appEntities.AppWorkCtx;

        var ser = jsonSerializer.SetApp(appCtx.AppReader);
        // Since we're importing directly into this app, we would prefer local content-types
        ser.PreferLocalAppTypes = true;

        #region check if it's an update, and do more security checks then - shared with EntitiesController.Save

        // basic permission checks
        var permCheck = saveSecurity.DoPreSaveSecurityCheck(context, package.Items);

        var foundItems = package.Items
            .Where(i => i.Entity.Id != 0 || i.Entity.Guid != Guid.Empty)
            .Select(i => i.Entity.Guid != Guid.Empty
                    ? appEntities.Get(i.Entity.Guid) // prefer guid access if available
                    : appEntities.Get(i.Entity.Id) // otherwise id
            );
        if (foundItems.Any(i => i != null) && !permCheck.EnsureAll(GrantSets.UpdateSomething, out var error))
            throw HttpException.PermissionDenied(error);

        #endregion


        var itemsToProcess = package.Items
            .Where(i => !i.Header.IsEmpty && i.Header.EditInfo?.ReadOnly != true)
            .ToList();

        l.A($"items to process: {itemsToProcess.Count} of {package.Items.Count}");

        var saveValidator = new SaveDataValidator(Log);
        var updateValidator = new SaveDataUpdateValidator(Log);
        var items = await Task.WhenAll(itemsToProcess
            .Select(async (i, index) => // index is helpful in case of errors
            {
                var ent = ser.Deserialize(i.Entity, false, false);

                // Check basic entity integrity
                var isOkException = saveValidator.EntityNotNullAndAttributeCountOk(index, ent);
                if (isOkException != null)
                    throw isOkException;

                // Check if Save is disabled because of content-type metadata (new v21)
                // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
                var (returnEntity, _ /* Decorator */, procException, processor) = await valContentTypeDataStore.Value
                    .PreSave(index, ent);

                if (procException != null)
                    throw procException;

                ent = returnEntity ?? throw HttpException.BadRequest($"Failed to process entity on index {index}");

                // If update check everything is ok, and if the ID needs to be reset
                var validatorResult = updateValidator.IfUpdateValidateAndCorrectIds(appEntities, index, ent);
                if (validatorResult.Exception != null)
                    throw validatorResult.Exception;

                // Reconstruct the entity, with possible ID reset, and with the correct owner and published state
                ent = dataBuilder.Entity.CreateFrom(ent,
                    id: validatorResult.ResetId,
                    isPublished: package.IsPublished, // the published state is only in the header, not per entity, so we need to set it here
                    owner: ent.Owner.NullIfNoValue() ?? context.User.IdentityToken
                );

                // new in 11.01
                if (i.Header.Parent != null)
                {
                    // Check if Add was true, and fix if it had already been saved (EntityId != 0)
                    // the entityId is reset by the validator if it turns out to be an update
                    // todo: verify use - maybe it's to set before we save, as maybe afterward it's always != 0?
                    var add = i.Header.AddSafe;
                    i.Header.Add = add;
                    if (ent.EntityId > 0 && add)
                        i.Header.Add = false;
                }

                return (
                    Bundle: new BundleWithHeader<IEntity>
                    {
                        Header = i.Header,
                        Entity = ent
                    },
                    Processor: processor
                );
            })
            .ToList());

        l.A("items to save generated, all data tests passed");

        if (!items.Any())
            return l.Return([], "returning: 0");

        var itemsWithoutProcessor = items
            .Select(i => i.Bundle)
            .ToList();

        var result = pagePublishing.SaveInPagePublishing(
            context,
            ctxService.BlockOrNull(),
            appId,
            itemsWithoutProcessor,
            partOfPage,
            forceSaveAsDraft => DoSave(appEntities, itemsWithoutProcessor, package.DraftShouldBranch || forceSaveAsDraft),
            permCheck
        );

        var itemsWithProcessor = items
            .Where(i => i.Processor != null)
            .ToList();

        if (!itemsWithProcessor.Any())
            return l.Return(result, $"returning: {result.Count}, no {DataProcessingEvents.PostSave}");

        l.A($"Will run processors for {itemsWithProcessor.Count} items");
        foreach (var item in itemsWithProcessor)
        {
            try
            {
                var post = await item.Processor!.Process(DataProcessingEvents.PostSave, new() { Data = item.Bundle.Entity });
            }
            catch (Exception ex)
            {
                l.Ex(ex, $"Error running processor for entity {item.Bundle.Entity.EntityGuid} ({item.Bundle.Entity.EntityId})");
            }
        }

        return l.Return(result, $"returning: {result.Count}");
    }


    private Dictionary<Guid, int> DoSave(WorkEntities workAppEntities, List<BundleWithHeader<IEntity>> items, bool forceSaveAsDraft)
    {
        // only save entities that are
        // a. not in a group
        // b. in a group where the slot isn't marked as empty
        var entitiesToSave = items
            .Where(e => !e.Header.IsContentBlockMode || !e.Header.IsEmpty)
            .ToList();

        saveBackendHelper.UpdateGuidAndPublishedAndSaveMany(workAppEntities.AppWorkCtx, entitiesToSave, forceSaveAsDraft);
        return saveBackendHelper.GenerateIdList(workAppEntities, items);
    }
}