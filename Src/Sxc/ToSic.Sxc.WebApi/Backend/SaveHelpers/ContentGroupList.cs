using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.SaveHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupList(
    GenWorkPlus<WorkBlocks> blocks,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    GenWorkDb<WorkFieldList> workFieldList)
    : ServiceBase("Api.GrpPrc", connect: [blocks, workFieldList, blockEditorSelectorLazy])
{
    #region Constructor / DI

    public ContentGroupList Init(IAppIdentity appIdentity)
    {
        _appIdentity = appIdentity;
        AppCtx = blocks.CtxSvc.Context(appIdentity);
        return this;
    }

    private IAppIdentity _appIdentity = null!;
    private IAppWorkCtx AppCtx { get; set; } = null!;
    #endregion

    internal bool IfChangesAffectListUpdateIt(IBlock? block, List<BundleWithHeader<IEntity>> items, Dictionary<Guid, int> ids)
    {
        var l = Log.Fn<bool>();
        var groupItems = items
            .Where(i => i.Header.Parent != null)
            .GroupBy(i => i.Header.Parent!.Value.ToString() + i.Header.IndexSafeOrFallback() + i.Header.AddSafe)
            .ToList();

        // if it's new, it has to be added to a group
        // only add if the header wants it, AND we started with ID unknown
        return groupItems.Any()
            ? l.Return(PostSaveUpdateIdsInParent(block, ids, groupItems), "post save ids")
            : l.Return(true, "no additional group processing necessary");
    }

    private bool PostSaveUpdateIdsInParent(IBlock? block, Dictionary<Guid, int> postSaveIds, IEnumerable<IGrouping<string, BundleWithHeader<IEntity>>> pairsOrSingleItems)
    {
        var l = Log.Fn<bool>($"{_appIdentity.AppId}");

        // If no content block given, skip all this
        if (block == null)
            return l.ReturnTrue("no block, nothing to update");


        foreach (var bundle in pairsOrSingleItems)
        {
            l.A("processing:" + bundle.Key);

            if (bundle.First().Header.Parent == null)
                continue;

            var parent = AppCtx.AppReader.GetDraftOrPublished(bundle.First().Header.GetParentEntityOrError())!;
            var targetIsContentBlock = parent.Type.Name == WorkBlocks.BlockTypeName;
                
            var primaryItem = targetIsContentBlock
                ? FindContentItem(bundle)
                : bundle.First();
            var primaryId = GetIdFromGuidOrError(postSaveIds, primaryItem.Entity.EntityGuid);

            var ids = targetIsContentBlock
                ? [primaryId, FindPresentationItem(postSaveIds, bundle)]
                : new[] {primaryId as int?};

            var index = primaryItem.Header.IndexSafeOrFallback();
            // fix https://github.com/2sic/2sxc/issues/2846 - Bug: Adding an item to a list doesn't seem to respect the position
            // This is used on new content item (+)
            var indexNullAddToEnd = primaryItem.Header.Index == null;
            var willAdd = primaryItem.Header.AddSafe;

            l.A($"will add: {willAdd}; Group.Add:{primaryItem.Header.Add}; EntityId:{primaryItem.Entity.EntityId}");

            var fieldPair = targetIsContentBlock
                ? ViewParts.PickFieldPair(primaryItem.Header.Field!)
                : [primaryItem.Header.Field!];

            var fieldList = workFieldList.New(AppCtx.AppReader);
            if (willAdd) // this cannot be auto-detected, it must be specified
            {

                // handle edge case on app with empty list, when index=1, but it should be index=0 (indexNullAddToEnd=true have the same effect)
                // fix https://github.com/2sic/2sxc/issues/2943 
                if (!parent.Children(fieldPair.First()).Any() && !targetIsContentBlock)
                    indexNullAddToEnd = true;
                    
                fieldList.FieldListAdd(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft, indexNullAddToEnd, targetIsContentBlock);
            }
            else
                fieldList.FieldListReplaceIfModified(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft);

        }

        // update-module-title
        blockEditorSelectorLazy.Value.GetEditor(block).UpdateTitle();
        return l.ReturnTrue("ok");
    }

    private static BundleWithHeader<T> FindContentItem<T>(IGrouping<string, BundleWithHeader<T>> bundle)
    {
        var primaryItem =
            bundle.FirstOrDefault(e => e.Header.Field.EqualsInsensitive(ViewParts.Content))
            ?? bundle.FirstOrDefault(e => e.Header.Field.EqualsInsensitive(ViewParts.FieldHeader));
        return primaryItem
               ?? throw new("unexpected group-entity assignment, cannot figure it out");
    }

    /// <summary>
    /// Get saved entity (to get its ID)
    /// </summary>
    private static int GetIdFromGuidOrError(IReadOnlyDictionary<Guid, int> postSaveIds, Guid guid) =>
        postSaveIds.TryGetValue(guid, out var id)
            ? id
            : throw new("Saved entity not found - not able to update BlockConfiguration");

    private int? FindPresentationItem(IReadOnlyDictionary<Guid, int> postSaveIds, IGrouping<string, BundleWithHeader<IEntity>> bundle)
    {
        var l = Log.Fn<int?>();
        int? presentationId = null;
        var presItem =
            bundle.FirstOrDefault(e => e.Header.Field.EqualsInsensitive(ViewParts.Presentation))
            ?? bundle.FirstOrDefault(e => e.Header.Field.EqualsInsensitive(ViewParts.ListPresentation));

        if (presItem == null)
            return l.ReturnNull("no presentation");
        // use null if it shouldn't have one
        if (presItem.Header.IsEmpty)
            return l.ReturnNull("header is empty");

        if (postSaveIds.TryGetValue(presItem.Entity.EntityGuid, out var id))
            presentationId = id;

        return l.Return(presentationId, "found");
    }

    internal List<ItemIdentifier> ConvertGroup(List<ItemIdentifier> identifiers)
    {
        var l = Log.Fn<List<ItemIdentifier>>();
        var result = identifiers
            .Select(identifier => identifier == null
                ? null
                : identifier with { IsContentBlockMode = DetectContentBlockMode(identifier) }
            )
            .ToList();
        //foreach (var identifier in identifiers.Where(identifier => identifier != null))
        //    identifier.IsContentBlockMode = DetectContentBlockMode(identifier);
        return l.Return(result);
    }

    internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> identifiers)
    {
        var lOuter = Log.Fn<List<ItemIdentifier>>();
         //new List<ItemIdentifier>();
        var appBlocks = blocks.New(AppCtx);
        var corrected = identifiers
            .Select((identifier, index) =>
            {
                var l = lOuter.Fn<ItemIdentifier>($"{index}");
                // Case one, it's a Content-Group - in this case the content-type name comes from View configuration
                if (identifier.IsContentBlockMode)
                {
                    if (!identifier.Parent.HasValue)
                        return l.ReturnNull("identifier is ContentBlockMode, but no parent");

                    var contentGroup = appBlocks.GetBlockConfig(identifier.GetParentEntityOrError());
                    var contentTypeName = contentGroup.View?.GetTypeStaticName(identifier.Field!) ?? "";

                    // if there is no content-type for this, then skip it (don't deliver anything)
                    if (contentTypeName == "")
                        return l.ReturnNull(
                            $"identifier is ContentBlockMode, but no content-type on field {identifier.Field}");

                    identifier = identifier with { ContentTypeName = contentTypeName };
                    identifier = ConvertListIndexToEntityIds(identifier, contentGroup);
                    return l.Return(identifier, $"identifier is ContentBlockMode, type is '{contentTypeName}'");
                }

                // Not Case #2
                // Default case - just a normal identifier
                if (identifier is not { Parent: not null, Field: not null })
                    return l.Return(identifier, "default case, normal identifier");

                // Case #2 it's an entity inside a field of another entity
                // Added in v11.01
                // check if it's a request for new, in which case we should check the create-type
                if (identifier.AddSafe)
                {
                    lOuter.A("identifier is new");
                    // look up type
                    var target = AppCtx.AppReader.List.GetOne(identifier.Parent.Value)!;
                    var field = target.Type[identifier.Field]!;
                    identifier = identifier with { ContentTypeName = new WorkAttributeEntityInspectType().PrimaryTypeName(field) }; 
                    return l.Return(identifier, "identifier is new");
                }

                // Otherwise it's an edit operation, and it could be a different content-type than is configured for the field
                // because the field may be configured for multiple types
                lOuter.A("identifier is edit");
                var realEntity = AppCtx.AppReader.List.GetOne(identifier.EntityId);
                if (realEntity == null)
                    return l.ReturnNull("identifier is edit, but couldn't find real entity.");
                identifier = identifier with { ContentTypeName = realEntity.Type.NameId };
                return l.Return(identifier, $"identity is edit, content-Type is {identifier.ContentTypeName}");

            })
            .Where(id => id != null)
            .Cast<ItemIdentifier>()
            .ToList();

        return lOuter.Return(corrected, $"{corrected.Count}");
    }


    /// <summary>
    /// Check if the save will affect a ContentBlock.
    /// If it's a simple entity-edit or edit of item inside a normal field list, it returns false
    /// </summary>
    /// <returns></returns>
    private bool DetectContentBlockMode(ItemIdentifier identifier)
    {
        var l = Log.Fn<bool>();
        if (!identifier.Parent.HasValue)
            return l.ReturnFalse("no parent");

        // get the entity and determine if it's a content-block. If yes, that should affect the differences in load/save
        var entity = AppCtx.AppReader.List.GetOne(identifier.Parent.Value)!;
        return l.Return(entity.Type.Name == WorkBlocks.BlockTypeName, "type name should match");
    }


    private ItemIdentifier ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration)
    {
        var l = Log.Fn<ItemIdentifier>();
        var part = blockConfiguration[identifier.Field!];
        if (!identifier.AddSafe) // not in add-mode
        {
            var idx = identifier.IndexSafeOrFallback(part.Count - 1);
            if (idx >= 0 && part.Count > idx && // has as many items as desired
                part[idx] != null) // and the slot has something
                identifier = identifier with { EntityId = part[idx].EntityId };
        }

        // tell the UI that it should not actually use this data yet, keep it locked
        if (!identifier.Field!.ToLowerInvariant().Contains(ViewParts.PresentationLower))
            return l.Return(identifier, "no presentation");

        // the following steps are only for presentation items
        identifier = identifier with { IsEmptyAllowed = true };

        if (identifier.EntityId != 0)
            return l.Return(identifier, "id != 0");

        identifier = identifier with { IsEmpty = true };

        var duplicate = identifier.Field.ToLowerInvariant() == ViewParts.PresentationLower
            ? blockConfiguration.View!.PresentationItem?.EntityId
            : blockConfiguration.View!.HeaderPresentationItem?.EntityId;
        identifier = identifier with { DuplicateEntity = duplicate };

        return l.Return(identifier,"ok");
    }
}