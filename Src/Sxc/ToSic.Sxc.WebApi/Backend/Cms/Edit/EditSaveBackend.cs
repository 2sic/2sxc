using ToSic.Eav.Data.Build;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Serialization.Internal;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.SaveHelpers;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EditSaveBackend(
    SxcPagePublishing pagePublishing,
    GenWorkPlus<WorkEntities> workEntities,
    ISxcContextResolver ctxResolver,
    JsonSerializer jsonSerializer,
    SaveSecurity saveSecurity,
    SaveEntities saveBackendHelper,
    DataBuilder dataBuilder)
    : ServiceBase("Cms.SaveBk",
        connect:
        [
            pagePublishing, workEntities, ctxResolver, jsonSerializer, saveSecurity, saveBackendHelper, dataBuilder
        ])
{
    #region DI Constructor and Init

    public EditSaveBackend Init(int appId)
    {
        _appId = appId;
        // The context should be from the block if there is one, because it affects saving/publishing
        // Basically it can result in things being saved draft or titles being updated
        _context = ctxResolver.GetBlockOrSetApp(appId);
        pagePublishing.Init(_context);
        return this;
    }

    private IContextOfApp _context;
    private int _appId;
    #endregion

    public Dictionary<Guid, int> Save(EditDto package, bool partOfPage)
    {
        Log.A($"save started with a#{_appId}, i⋮{package.Items.Count}, partOfPage:{partOfPage}");

        var validator = new SaveDataValidator(package, Log);
        // perform some basic validation checks
        var containsOnlyExpectedNodesException = validator.ContainsOnlyExpectedNodes();
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
        var appEntities = workEntities.New(_appId);
        var appCtx = appEntities.AppWorkCtx;

        var ser = jsonSerializer.SetApp(appCtx.AppReader);
        // Since we're importing directly into this app, we would prefer local content-types
        ser.PreferLocalAppTypes = true;
        validator.PrepareForEntityChecks(appEntities);

        #region check if it's an update, and do more security checks then - shared with EntitiesController.Save
        // basic permission checks
        var permCheck = saveSecurity.Init(_context).DoPreSaveSecurityCheck(package.Items);

        var foundItems = package.Items.Where(i => i.Entity.Id != 0 || i.Entity.Guid != Guid.Empty)
            .Select(i => i.Entity.Guid != Guid.Empty
                    ? appEntities.Get(i.Entity.Guid) // prefer guid access if available
                    : appEntities.Get(i.Entity.Id) // otherwise id
            );
        if (foundItems.Any(i => i != null) && !permCheck.EnsureAll(GrantSets.UpdateSomething, out var error))
            throw HttpException.PermissionDenied(error);
        #endregion


        var items = package.Items.Where(i => !i.Header.IsEmpty).Select(i =>
            {
                var ent = ser.Deserialize(i.Entity, false, false);

                var index = package.Items.IndexOf(i); // index is helpful in case of errors
                var isOkException = validator.EntityIsOk(index, ent);
                if (isOkException != null)
                    throw isOkException;

                var resultValidator = validator.IfUpdateValidateAndCorrectIds(index, ent);

                if (resultValidator.Exception != null)
                    throw resultValidator.Exception;

                ent = dataBuilder.Entity.CreateFrom(ent, id: resultValidator.ResetId, isPublished: package.IsPublished,
                    placeDraftInBranch: package.DraftShouldBranch);

                //ent.ResetEntityId(resultValidator.ResetId ?? 0); //AjaxPreviewHelperWIP!
                //ent.IsPublished = package.IsPublished;
                //ent.PlaceDraftInBranch = package.DraftShouldBranch;

                // new in 11.01
                if (i.Header.Parent != null)
                {
                    // Check if Add was true, and fix if it had already been saved (EntityId != 0)
                    // the entityId is reset by the validator if it turns out to be an update
                    // todo: verify use - maybe it's to set before we save, as maybe afterwards it's always != 0?
                    var add = i.Header.AddSafe;
                    i.Header.Add = add;
                    if (ent.EntityId > 0 && add) i.Header.Add = false;
                }

                return new BundleWithHeader<IEntity>
                {
                    Header = i.Header,
                    Entity = ent
                };
            })
            .ToList();

        Log.A("items to save generated, all data tests passed");

        return pagePublishing.SaveInPagePublishing(ctxResolver.BlockOrNull(), _appId, items, partOfPage,
            forceSaveAsDraft => DoSave(appEntities, items, forceSaveAsDraft),
            permCheck);
    }


    private Dictionary<Guid, int> DoSave(WorkEntities workEntities, List<BundleWithHeader<IEntity>> items, bool forceSaveAsDraft)
    {
        // only save entities that are
        // a) not in a group
        // b) in a group where the slot isn't marked as empty
        var entitiesToSave = items
            .Where(e => !e.Header.IsContentBlockMode || !e.Header.IsEmpty)
            .ToList();

        saveBackendHelper.UpdateGuidAndPublishedAndSaveMany(workEntities.AppWorkCtx, entitiesToSave, forceSaveAsDraft);
        return saveBackendHelper.GenerateIdList(workEntities, items);
    }
}