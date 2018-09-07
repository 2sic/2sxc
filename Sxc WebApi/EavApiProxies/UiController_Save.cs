using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Interfaces;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)] // while in dev-mode, only for super-users
    public partial class UiController
    {
        // todo: replace object with Dictionary<Guid, int> when ready
        [HttpPost]
        public object Save([FromBody] AllInOne package, int appId, bool partOfPage)
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
            var ser = new JsonSerializer(appRead.Package, Log);
            validator.PrepareForEntityChecks(appRead);

            // permission checks
            var permCheck = new SaveHelpers.Security(SxcInstance, Log).DoSaveSecurityCheck(appId, package.Items);

            var items = package.Items.Select(i =>
            {
                var ent = ser.Deserialize(i.Entity, false, false);
                if (!validator.EntityIsOk(package.Items.IndexOf(i) , ent, out exp))
                    throw exp;

                return new BundleWithHeader<IEntity>
                {
                    Header = i.Header,
                    Entity = ent
                };
            })
            .ToList();

            Log.Add("itemsToSave generated, all data tests passed");

            //Dictionary<Guid, int> SaveOldWithGroups(bool forceSaveAsDraft) =>
            //    SaveNewWrapper(appMan, items, forceSaveAsDraft);

            return new SaveHelpers.DnnPublishing(SxcInstance, Log)
                .SaveWithinDnnPagePublishing(appId, items, partOfPage,
                    forceSaveAsDraft => SaveNewWrapper(appMan, items, forceSaveAsDraft),
                    permCheck);



            // todo: update group if this was added / changed

            return "call to save worked - save doesn't work yet";
        }

        public Dictionary<Guid, int> SaveNewWrapper(AppManager appMan, List<BundleWithHeader<IEntity>> items, bool draftOnly)
        {
            var entitySetToImport = items
                .Where(entity => entity.Header.Group == null || !entity.Header.Group.SlotIsEmpty)
                .ToList();

            var eavEntitiesController = new Eav.WebApi.EntitiesController(Log);

            return eavEntitiesController
                .UpdateGuidAndPublishedAndSaveMany(appMan, items, entitySetToImport, draftOnly);

        }

    }
}
