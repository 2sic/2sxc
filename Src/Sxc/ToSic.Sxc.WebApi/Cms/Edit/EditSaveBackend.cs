﻿using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Save;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EditSaveBackend : WebApiBackendBase<EditSaveBackend>
    {

        #region DI Constructor and Init

        public EditSaveBackend(SxcPagePublishing pagePublishing, Lazy<AppManager> appManagerLazy, IServiceProvider serviceProvider, IContextResolver ctxResolver) : base(serviceProvider, "Cms.SaveBk")
        {
            _pagePublishing = pagePublishing;
            _appManagerLazy = appManagerLazy;
            _ctxResolver = ctxResolver;
        }
        private readonly SxcPagePublishing _pagePublishing;
        private readonly Lazy<AppManager> _appManagerLazy;
        private readonly IContextResolver _ctxResolver;

        public EditSaveBackend Init(int appId, ILog log)
        {
            Init(log);
            _appId = appId;
            // The context should be from the block if there is one, because it affects saving/publishing
            // Basically it can result in things being saved draft or titles being updated
            _context = _ctxResolver.BlockOrApp(appId);
            _pagePublishing.Init(_context, Log);
            return this;
        }

        private IContextOfApp _context;
        private int _appId;
        #endregion

        public Dictionary<Guid, int> Save(EditDto package, bool partOfPage)
        {
            Log.Add($"save started with a#{_appId}, i⋮{package.Items.Count}, partOfPage:{partOfPage}");

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

            var appMan = _appManagerLazy.Value.Init(_appId, Log);
            var appRead = appMan.Read;
            var ser = ServiceProvider.Build<JsonSerializer>().Init(appRead.AppState, Log);
            // Since we're importing directly into this app, we would prefer local content-types
            ser.PreferLocalAppTypes = true;
            validator.PrepareForEntityChecks(appRead);

            #region check if it's an update, and do more security checks then - shared with EntitiesController.Save
            // basic permission checks
            var permCheck = new Save.SaveSecurity(_context, Log)
                .DoPreSaveSecurityCheck(_appId, package.Items);

            var foundItems = package.Items.Where(i => i.Entity.Id != 0 && i.Entity.Guid != Guid.Empty)
                .Select(i => i.Entity.Guid != Guid.Empty
                        ? appRead.Entities.Get(i.Entity.Guid) // prefer guid access if available
                        : appRead.Entities.Get(i.Entity.Id)  // otherwise id
                );
            if (foundItems.Any(i => i != null) && !permCheck.EnsureAll(GrantSets.UpdateSomething, out var error))
                throw HttpException.PermissionDenied(error);
            #endregion


            var items = package.Items.Select(i =>
            {
                var ent = (Entity) ser.Deserialize(i.Entity, false, false);

                var index = package.Items.IndexOf(i); // index is helpful in case of errors
                if (!validator.EntityIsOk(index, ent, out exp))
                    throw exp;

                if (!validator.IfUpdateValidateAndCorrectIds(index, ent, out exp))
                    throw exp;

                ent.IsPublished = package.IsPublished;
                ent.PlaceDraftInBranch = package.DraftShouldBranch;

                // new in 11.01
                if (i.Header.ListHas())
                {
                    // Check if Add was true, and fix if it had already been saved (EntityId != 0)
                    // the entityId is reset by the validator if it turns out to be an update
                    // todo: verify use - maybe it's to set before we save, as maybe afterwards it's always != 0?
                    var add = i.Header.ListAdd();
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

            Log.Add("items to save generated, all data tests passed");

            return _pagePublishing.SaveInPagePublishing(_ctxResolver.RealBlockOrNull(), _appId, items, partOfPage,
                    forceSaveAsDraft => DoSave(appMan, items, forceSaveAsDraft),
                    permCheck);
        }


        private Dictionary<Guid, int> DoSave(AppManager appMan, List<BundleWithHeader<IEntity>> items, bool forceSaveAsDraft)
        {
            // only save entities that are
            // a) not in a group
            // b) in a group where the slot isn't marked as empty
            var entitiesToSave = items
                .Where(e => e.Header.Group == null || !e.Header.Group.SlotIsEmpty)
                .ToList();

            var save = new Eav.WebApi.SaveHelpers.SaveEntities(Log);
            save.UpdateGuidAndPublishedAndSaveMany(appMan, entitiesToSave, forceSaveAsDraft);

            return save.GenerateIdList(appMan.Read.Entities, items);

        }
    }
}
