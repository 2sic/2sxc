using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;
using static System.StringComparison;
using BlockEditorBase = ToSic.Sxc.Blocks.Edit.BlockEditorBase;

namespace ToSic.Sxc.WebApi
{
    internal class ContentGroupList: SaveHelperBase
    {
        public ContentGroupList(IBlockBuilder blockBuilder, ILog parentLog) : base(blockBuilder, parentLog, "Api.GrpPrc") {}

        internal bool IfChangesAffectListUpdateIt(int appId, List<BundleWithHeader<IEntity>> items, Dictionary<Guid, int> ids)
        {
            var wrapLog = Log.Call<bool>();
            var groupItems = items.Where(i => i.Header.ListHas())
                .GroupBy(i => i.Header.ListParent().ToString() + i.Header.ListIndex() + i.Header.ListAdd())
                .ToList();

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            return groupItems.Any() 
                ? wrapLog(null, PostSaveUpdateIdsInParent(appId, ids, groupItems)) 
                : wrapLog("no additional group processing necessary", true);
        }

        private BlockConfiguration GetBlockConfig(IAppIdentity app, Guid blockGuid)
            => new CmsRuntime(app, Log, BlockBuilder.UserMayEdit,
                BlockBuilder.Environment.PagePublishing.IsEnabled(BlockBuilder.Container.Id)).Blocks.GetBlockConfig(blockGuid);

        private bool PostSaveUpdateIdsInParent(
            int appId,
            Dictionary<Guid, int> postSaveIds,
            IEnumerable<IGrouping<string, BundleWithHeader<IEntity>>> pairsOrSingleItems)
        {
            var wrapLog = Log.Call<bool>($"{appId}");
            var app = Factory.Resolve<Apps.App>().Init(new AppIdentity(Eav.Apps.App.AutoLookupZone, appId), 
                ConfigurationProvider.Build(BlockBuilder, true), false, Log);

            foreach (var bundle in pairsOrSingleItems)
            {
                Log.Add("processing:" + bundle.Key);
                var entity = app.Data.List.One(bundle.First().Header.ListParent());
                var targetIsContentBlock = entity.Type.Name == BlocksRuntime.BlockTypeName;
                
                var primaryItem = targetIsContentBlock ? FindContentItem(bundle) : bundle.First();
                var primaryId = GetIdFromGuidOrError(postSaveIds, primaryItem.Entity.EntityGuid);

                var ids = targetIsContentBlock
                    ? new[] {primaryId, FindPresentationItem(postSaveIds, bundle)}
                    : new[] {primaryId as int?};

                var index = primaryItem.Header.ListIndex();

                // add or update slots
                //var itemIsReallyNew = primaryItem.EntityId == 0; // only really add if it's really new
                var willAdd = primaryItem.Header.ListAdd();// && itemIsReallyNew;

                // 2019-07-01 2dm needed to add this, because new-save already gives it an ID
                //if (primaryItem.Header.ReallyAddBecauseAlreadyVerified != null)
                //    willAdd = primaryItem.Header.ReallyAddBecauseAlreadyVerified.Value;

                Log.Add($"will add: {willAdd}; " + // add-pre-verified:{primaryItem.Header.ReallyAddBecauseAlreadyVerified}; " +
                        $"Group.Add:{primaryItem.Header.Add}; EntityId:{primaryItem.Entity.EntityId}");

                var cms = new CmsManager(app, Log);
                var fieldPair = targetIsContentBlock
                    ? ViewParts.PickPair(primaryItem.Header.Group.Part)
                    : new[] {primaryItem.Header.Field};

                if (willAdd) // this cannot be auto-detected, it must be specified
                    cms.Entities.FieldListAdd(entity, fieldPair, index, ids, cms.EnablePublishing);
                else
                    cms.Entities.FieldListReplaceIfModified(entity, fieldPair, index, ids, cms.EnablePublishing);

            }

            // update-module-title
            BlockEditorBase.GetEditor(BlockBuilder).UpdateTitle();
            return wrapLog("ok", true);
        }

        private static BundleWithHeader<T> FindContentItem<T>(IGrouping<string, BundleWithHeader<T>> bundle)
        {
            var primaryItem = bundle.FirstOrDefault(e => string.Equals(e.Header.Group.Part, ViewParts.Content, OrdinalIgnoreCase)) 
                   ?? bundle.FirstOrDefault(e => string.Equals(e.Header.Group.Part, ViewParts.ListContent, OrdinalIgnoreCase));
            if (primaryItem == null)
                throw new Exception("unexpected group-entity assignment, cannot figure it out");
            return primaryItem;
        }

        /// <summary>
        /// Get saved entity (to get its ID)
        /// </summary>
        private static int GetIdFromGuidOrError(Dictionary<Guid, int> postSaveIds, Guid guid)
        {
            if (!postSaveIds.ContainsKey(guid))
                throw new Exception("Saved entity not found - not able to update BlockConfiguration");

            return postSaveIds[guid];
        }

        private static int? FindPresentationItem(Dictionary<Guid, int> postSaveIds, IGrouping<string, BundleWithHeader<IEntity>> bundle)
        {
            int? presentationId = null;
            var presItem =
                bundle.FirstOrDefault(e => string.Equals(e.Header.Group.Part, ViewParts.Presentation, OrdinalIgnoreCase))
                ?? bundle.FirstOrDefault(e =>
                    string.Equals(e.Header.Group.Part, ViewParts.ListPresentation, OrdinalIgnoreCase));

            if (presItem == null) return null;

            if (postSaveIds.ContainsKey(presItem.Entity.EntityGuid))
                presentationId = postSaveIds[presItem.Entity.EntityGuid];

            presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId;
            // use null if it shouldn't have one

            return presentationId;
        }

        internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> identifiers, IAppIdentity app)
        {
            var wrapLog = Log.Call<List<ItemIdentifier>>();
            var newItems = new List<ItemIdentifier>();
            foreach (var identifier in identifiers)
            {
                // Case one, it's a Content-Group (older model, probably drop soon)
                if (identifier.Group != null)
                {
                    var contentGroup = GetBlockConfig(app, identifier.Group.Guid);
                    var contentTypeStaticName = (contentGroup.View as View)?
                                                .GetTypeStaticName(identifier.Group.Part) ?? "";

                    // if there is no content-type for this, then skip it (don't deliver anything)
                    if (contentTypeStaticName == "")
                        continue;

                    identifier.ContentTypeName = contentTypeStaticName;
                    ConvertListIndexToEntityIds(identifier, contentGroup);
                    newItems.Add(identifier);
                    continue;
                }

                // New in v11.01
                if (identifier.Parent != null && identifier.Field != null)
                {
                    // look up type
                    var target = BlockBuilder.App.Data.List.One(identifier.Parent.Value);
                    var field = target.Type[identifier.Field];
                    identifier.ContentTypeName = field.EntityFieldItemTypePrimary();
                    newItems.Add(identifier);
                    continue;
                }

                // Default case - just a normal identifier
                newItems.Add(identifier);
            }
            return wrapLog(null, newItems);
        }


        private static void ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration)
        {
            var part = blockConfiguration[identifier.Group.Part];
            if (!identifier.ListAdd()) // not in add-mode
            {
                var idx = identifier.ListIndex();
                if(part.Count > idx && // has as many items as desired
                   part[idx] != null) // and the slot has something
                    identifier.EntityId = part[idx].EntityId;
            }

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
