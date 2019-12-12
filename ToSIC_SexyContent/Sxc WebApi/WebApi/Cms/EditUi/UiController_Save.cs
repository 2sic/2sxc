using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Dnn;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class UiController
    {
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOne package, int appId, bool partOfPage)
        {
            Log.Add($"save started with a#{appId}, i⋮{package.Items.Count}, partOfPage:{partOfPage}");

            var validator = new SaveDataValidator(package, Log);
            // perform some basic validation checks
            if (!validator.ContainsOnlyExpectedNodes(out var exp))
                throw exp;

            // todo: unsure about this - thought I should check contentblockappid in group-header, because this is where it should be saved!
            //var contextAppId = appId;
            //var targetAppId = package.Items.First().Header.Group.ContentBlockAppId;
            //if (targetAppId != 0)
            //{
            //    Log.Add($"detected content-block app to use: {targetAppId}; in context of app {contextAppId}");
            //    appId = targetAppId;
            //}

            var appMan = new AppManager(appId, Log);
            var appRead = appMan.Read;
            var ser = new JsonSerializer(appRead.AppState, Log)
            {
                // Since we're importing directly into this app, we would prefer local content-types
                PreferLocalAppTypes = true
            };
            validator.PrepareForEntityChecks(appRead);

            #region check if it's an update, and do more security checks then - shared with EntitiesController.Save
            // basic permission checks
            var permCheck = new Security.Security(CmsBlock, Log)
                .DoPreSaveSecurityCheck(appId, package.Items);

            var foundItems = package.Items.Where(i => i.EntityId != 0 && i.EntityGuid != Guid.Empty)
                .Select(i => i.EntityGuid != Guid.Empty
                        ? appRead.Entities.Get(i.EntityGuid) // prefer guid access if available
                        : appRead.Entities.Get(i.EntityId)  // otherwise id
                );
            if (foundItems.Any(i => i != null) && !permCheck.EnsureAll(GrantSets.UpdateSomething, out var exception))
                throw exception;
            #endregion


            var items = package.Items.Select(i =>
            {
                var ent = ser.Deserialize(i.Entity, false, false) as Entity;
                
                var index = package.Items.IndexOf(i); // index is helpful in case of errors
                if (!validator.EntityIsOk(index, ent, out exp))
                    throw exp;

                if (!validator.IfUpdateValidateAndCorrectIds(index, ent, out exp))
                    throw exp;

                ent.IsPublished = package.IsPublished;
                ent.PlaceDraftInBranch = package.DraftShouldBranch;

                // only do this if we're adding to a group
                if (i.Header.Group != null)
                {
                    // the entityId is reset by the validator if it turns out to be an update
                    if (ent.EntityId > 0 && i.Header.Group.Add)
                        i.Header.Group.Add = false;

                    i.Header.Group.ReallyAddBecauseAlreadyVerified = i.Header.Group.Add;
                }

                return new BundleWithHeader<IEntity>
                {
                    Header = i.Header,
                    Entity = ent
                };
            })
            .ToList();

            Log.Add("items to save generated, all data tests passed");

            return new DnnPublishing(CmsBlock, Log)
                .SaveWithinDnnPagePublishing(appId, items, partOfPage,
                    forceSaveAsDraft => DoSave(appMan, items, forceSaveAsDraft),
                    permCheck);
        }

        public Dictionary<Guid, int> DoSave(AppManager appMan, List<BundleWithHeader<IEntity>> items, bool forceSaveAsDraft)
        {
            var entitySetToImport = items
                .Where(entity => entity.Header.Group == null || !entity.Header.Group.SlotIsEmpty)
                .ToList();

            var save = new Eav.WebApi.SaveHelpers.SaveEntities(Log);
            save.UpdateGuidAndPublishedAndSaveMany(appMan, entitySetToImport, forceSaveAsDraft);

            return save.GenerateIdList(appMan.Read.Entities, items);

        }

    }
}
