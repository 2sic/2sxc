using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Edit;
using static System.StringComparison;
using BlockEditorBase = ToSic.Sxc.Blocks.Edit.BlockEditorBase;

namespace ToSic.Sxc.WebApi.Save
{
    public class ContentGroupList: HasLog
    {
        #region Constructor / DI

        private readonly Lazy<CmsManager> _cmsManagerLazy;
        private readonly Generator<BlockEditorForModule> _blkEdtForMod;
        private readonly Generator<BlockEditorForEntity> _blkEdtForEnt;
        private CmsManager CmsManager => _cmsManager ?? (_cmsManager = _cmsManagerLazy.Value.Init(_appIdentity, _withDrafts, Log));
        private CmsManager _cmsManager;
        private bool _withDrafts = false;

        public ContentGroupList(Lazy<CmsManager> cmsManagerLazy, 
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt
            ) : base("Api.GrpPrc")
        {
            _cmsManagerLazy = cmsManagerLazy;
            _blkEdtForMod = blkEdtForMod;
            _blkEdtForEnt = blkEdtForEnt;
        }

        public ContentGroupList Init(IAppIdentity appIdentity, ILog parentLog, bool withDraftsTemp)
        {
            Log.LinkTo(parentLog);
            _appIdentity = appIdentity;
            _withDrafts = withDraftsTemp;
            return this;
        }

        private IAppIdentity _appIdentity;
        #endregion

        internal bool IfChangesAffectListUpdateIt(IBlock block, List<BundleWithHeader<IEntity>> items, Dictionary<Guid, int> ids)
        {
            var wrapLog = Log.Fn<bool>();
            var groupItems = items
                .Where(i => i.Header.ListHas())
                .GroupBy(i => i.Header.ListParent().ToString() + i.Header.ListIndex() + i.Header.ListAdd())
                .ToList();

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            return groupItems.Any() 
                ? wrapLog.Return(PostSaveUpdateIdsInParent(block, ids, groupItems)) 
                : wrapLog.ReturnTrue("no additional group processing necessary");
        }

        private bool PostSaveUpdateIdsInParent(IBlock block,
            Dictionary<Guid, int> postSaveIds,
            IEnumerable<IGrouping<string, BundleWithHeader<IEntity>>> pairsOrSingleItems)
        {
            var wrapLog = Log.Fn<bool>($"{_appIdentity.AppId}");

            if (block == null) return wrapLog.ReturnTrue("no block, nothing to update");

            // todo: if no block given, skip all this

            foreach (var bundle in pairsOrSingleItems)
            {
                Log.A("processing:" + bundle.Key);
                var entity = CmsManager.Read.AppState.List.One(bundle.First().Header.ListParent());
                var targetIsContentBlock = entity.Type.Name == BlocksRuntime.BlockTypeName;
                
                var primaryItem = targetIsContentBlock ? FindContentItem(bundle) : bundle.First();
                var primaryId = GetIdFromGuidOrError(postSaveIds, primaryItem.Entity.EntityGuid);

                var ids = targetIsContentBlock
                    ? new[] {primaryId, FindPresentationItem(postSaveIds, bundle)}
                    : new[] {primaryId as int?};

                var index = primaryItem.Header.ListIndex();
                var indexNullAddToEnd = primaryItem.Header.Index == null;
                var willAdd = primaryItem.Header.ListAdd();

                Log.A($"will add: {willAdd}; Group.Add:{primaryItem.Header.Add}; EntityId:{primaryItem.Entity.EntityId}");

                var fieldPair = targetIsContentBlock
                    ? ViewParts.PickFieldPair(primaryItem.Header.Group.Part)
                    : new[] {primaryItem.Header.Field};

                if (willAdd) // this cannot be auto-detected, it must be specified
                    CmsManager.Entities.FieldListAdd(entity, fieldPair, index, ids, block.Context.Publishing.ForceDraft, indexNullAddToEnd);
                else
                    CmsManager.Entities.FieldListReplaceIfModified(entity, fieldPair, index, ids, block.Context.Publishing.ForceDraft);

            }

            // update-module-title
            BlockEditorBase.GetEditor(block, _blkEdtForMod, _blkEdtForEnt).UpdateTitle();
            return wrapLog.ReturnTrue("ok");
        }

        private static BundleWithHeader<T> FindContentItem<T>(IGrouping<string, BundleWithHeader<T>> bundle)
        {
            var primaryItem = bundle.FirstOrDefault(e => string.Equals(e.Header.Group.Part, ViewParts.Content, OrdinalIgnoreCase)) 
                   ?? bundle.FirstOrDefault(e => string.Equals(e.Header.Group.Part, ViewParts.FieldHeader, OrdinalIgnoreCase));
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

        internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> identifiers)
        {
            var wrapLog = Log.Fn<List<ItemIdentifier>>();
            var newItems = new List<ItemIdentifier>();
            foreach (var identifier in identifiers)
            {
                // Case one, it's a Content-Group (older model, probably drop soon)
                if (identifier.Group != null)
                {
                    var contentGroup = CmsManager.Read.Blocks.GetBlockConfig(identifier.Group.Guid);
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
                    var target = CmsManager.Read.AppState.List.One(identifier.Parent.Value);
                    var field = target.Type[identifier.Field];
                    identifier.ContentTypeName = field.EntityFieldItemTypePrimary();
                    newItems.Add(identifier);
                    continue;
                }

                // Default case - just a normal identifier
                newItems.Add(identifier);
            }
            return wrapLog.Return(newItems);
        }


        private static void ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration)
        {
            var part = blockConfiguration[identifier.Group.Part];
            if (!identifier.ListAdd()) // not in add-mode
            {
                var idx = identifier.ListIndex(part.Count - 1);
                if(idx >= 0 && part.Count > idx && // has as many items as desired
                   part[idx] != null) // and the slot has something
                    identifier.EntityId = part[idx].EntityId;
            }

            // tell the UI that it should not actually use this data yet, keep it locked
            if (!identifier.Group.Part.ToLowerInvariant().Contains(ViewParts.PresentationLower))
                return;

            // the following steps are only for presentation items
            identifier.Group.SlotCanBeEmpty = true; // all presentations can always be locked

            if (identifier.EntityId != 0)
                return;

            identifier.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with

            identifier.DuplicateEntity =
                identifier.Group.Part.ToLowerInvariant() == ViewParts.PresentationLower
                    ? blockConfiguration.View.PresentationItem?.EntityId
                    : blockConfiguration.View.HeaderPresentationItem?.EntityId;
        }

    }
}
