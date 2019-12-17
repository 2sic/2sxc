using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.LookUp;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi
{
    internal class ContentGroupList: SaveHelperBase
    {
        public ContentGroupList(ICmsBlock cmsInstance, ILog parentLog) : base(cmsInstance, parentLog, "Api.GrpPrc") {}

        internal void IfInListUpdateList<T>(int appId, List<BundleWithHeader<T>> items, Dictionary<Guid, int> ids)
        {
            Log.Add("check groupings");
            var groupItems = items.Where(i => i.Header.Group != null)
                .GroupBy(i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add)
                .ToList();

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            if (groupItems.Any())
                UpdateList(appId, ids, groupItems);
            else
                Log.Add("no additional group processing necessary");
        }

        private BlockConfiguration GetBlockConfig(IApp app, Guid blockGuid)
            => new CmsRuntime(app, Log, CmsInstance.UserMayEdit,
                CmsInstance.Environment.PagePublishing.IsEnabled(CmsInstance.Container.Id)).Blocks.GetBlockConfig(blockGuid);

        private void UpdateList<T>(
            int appId,
            Dictionary<Guid, int> postSaveIds,
            IEnumerable<IGrouping<string, BundleWithHeader<T>>> groupItems)
        {
            var wrapLog = Log.Call($"{appId}");
            var app = new Apps.App(new DnnTenant(PortalSettings.Current), Eav.Apps.App.AutoLookupZone, appId,
                ConfigurationProvider.Build(CmsInstance, true), false, Log);

            foreach (var entitySets in groupItems)
            {
                Log.Add("processing:" + entitySets.Key);
                var contItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == ViewParts.ContentLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == ViewParts.ListContentLower);
                if (contItem == null)
                    throw new Exception("unexpected group-entity assignment, cannot figure it out");

                var presItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == ViewParts.PresentationLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == ViewParts.ListPresentationLower);

                // Get group to assign to and parameters
                var contentGroup = GetBlockConfig(app, contItem.Header.Group.Guid);// app.BlocksManager.GetBlockConfig(contItem.Header.Group.Guid);
                var partName = contItem.Header.Group.Part;

                // var part = blockConfiguration[partName];
                var index = contItem.Header.Group.Index;

                // Get saved entity (to get its ID)
                if (!postSaveIds.ContainsKey(contItem.EntityGuid))
                    throw new Exception("Saved entity not found - not able to update BlockConfiguration");

                var postSaveId = postSaveIds[contItem.EntityGuid];

                int? presentationId = null;

                if (presItem != null)
                {
                    if (postSaveIds.ContainsKey(presItem.EntityGuid))
                        presentationId = postSaveIds[presItem.EntityGuid];

                    presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId;
                    // use null if it shouldn't have one
                }

                // add or update slots
                var itemIsReallyNew = contItem.EntityId == 0; // only really add if it's really new
                var willAdd = contItem.Header.Group.Add && itemIsReallyNew;

                // 2019-07-01 2dm needed to add this, because new-save already gives it an ID
                if (contItem.Header.Group.ReallyAddBecauseAlreadyVerified != null)
                    willAdd = contItem.Header.Group.ReallyAddBecauseAlreadyVerified.Value;

                Log.Add($"will add: {willAdd}; add-pre-verified:{contItem.Header.Group.ReallyAddBecauseAlreadyVerified}; Group.Add:{contItem.Header.Group.Add}; {nameof(itemIsReallyNew)}:{itemIsReallyNew}; EntityId:{contItem.EntityId}");

                var cms = new CmsManager(app, Log);

                if (willAdd) // this cannot be auto-detected, it must be specified
                    /*contentGroup*/cms.Blocks .AddContentAndPresentationEntity(contentGroup, partName, index, postSaveId, presentationId);
                else
                    /*contentGroup*/cms.Blocks.UpdateEntityIfChanged(contentGroup, partName, index, postSaveId, true, presentationId);
            }

            // update-module-title
            CmsInstance.Block.Editor.UpdateTitle();
            wrapLog("ok");
        }

        internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> identifiers, IApp app)
        {
            Log.Add("ConvertListIndexToId()");
            var newItems = new List<ItemIdentifier>();
            foreach (var identifier in identifiers)
            {
                // only do special processing if it's a "group" item
                if (identifier.Group == null)
                {
                    newItems.Add(identifier);
                    continue;
                }

                var contentGroup = GetBlockConfig(app, identifier.Group.Guid);//  app.BlocksManager.GetBlockConfig(identifier.Group.Guid);
                var contentTypeStaticName = (contentGroup.View as View)?.GetTypeStaticName(identifier.Group.Part) ?? "";

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                identifier.ContentTypeName = contentTypeStaticName;

                ConvertListIndexToEntityIds(identifier, contentGroup);

                newItems.Add(identifier);
            }
            return newItems;
        }


        private static void ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration)
        {
            var part = blockConfiguration[identifier.Group.Part];
            if (!identifier.Group.Add && // not in add-mode
                part.Count > identifier.Group.Index && // has as many items as desired
                part[identifier.Group.Index] != null) // and the slot has something
                identifier.EntityId = part[identifier.Group.Index].EntityId;

            // tell the UI that it should not actually use this data yet, keep it locked
            if (!identifier.Group.Part.ToLower().Contains(ViewParts.PresentationLower))
                return;

            // the following steps are only for presentation items
            identifier.Group.SlotCanBeEmpty = true; // all presentations can always be locked

            if (identifier.EntityId != 0)
                return;

            identifier.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with

            identifier.DuplicateEntity =
                identifier.Group.Part.ToLower() == ViewParts.PresentationLower
                    ? blockConfiguration.View.PresentationItem?.EntityId
                    : blockConfiguration.View.HeaderPresentationItem?.EntityId;
        }

    }
}
