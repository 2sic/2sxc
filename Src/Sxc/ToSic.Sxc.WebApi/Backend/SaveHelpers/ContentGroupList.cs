using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.ListSlots.Sys;
using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.SaveHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupList(Generator<BlockUpdateTitle> blockEditor, AppWorkChain<WorkListSlots> workFieldList)
    : ServiceWithSetup<IAppWorkContext>("Api.GrpPrc", connect: [workFieldList, blockEditor])
{

    internal async Task<bool> IfChangesAffectListUpdateIt(IBlock? block, List<BundleWithHeader<IEntity>> items,
        Dictionary<Guid, int> ids)
    {
        var l = Log.Fn<bool>();
        var groupItems = items
            .Where(i => i.Header.Parent != null)
            .GroupBy(i => i.Header.Parent!.Value.ToString() + i.Header.IndexSafeOrFallback() + i.Header.AddSafe)
            .ToList();

        // if it's new, it has to be added to a group
        // only add if the header wants it, AND we started with ID unknown
        return groupItems.Any()
            ? l.Return(await PostSaveUpdateIdsInParent(block, ids, groupItems), "post save ids")
            : l.ReturnTrue("no additional group processing necessary");
    }

    private async Task<bool> PostSaveUpdateIdsInParent(IBlock? block, Dictionary<Guid, int> postSaveIds,
        IEnumerable<IGrouping<string, BundleWithHeader<IEntity>>> pairsOrSingleItems)
    {
        var l = Log.Fn<bool>($"{MyOptions.AppReader.AppId}");

        // If no content block given, skip all this
        if (block == null)
            return l.ReturnTrue("no block, nothing to update");


        foreach (var bundle in pairsOrSingleItems)
        {
            l.A("processing:" + bundle.Key);

            if (bundle.First().Header.Parent == null)
                continue;

            var parent = MyOptions.AppReader.GetDraftOrPublished(bundle.First().Header.GetParentEntityOrError())!;
            var targetIsContentBlock = parent.Type.Name == AppConstants.ContentGroupTypeName;

            var primaryItem = targetIsContentBlock
                ? FindContentItem(bundle)
                : bundle.First();
            var primaryId = GetIdFromGuidOrError(postSaveIds, primaryItem.Entity.EntityGuid);

            var ids = targetIsContentBlock
                ? [primaryId, FindPresentationItem(postSaveIds, bundle)]
                : new[] { primaryId as int? };

            var index = primaryItem.Header.IndexSafeOrFallback();
            // fix https://github.com/2sic/2sxc/issues/2846 - Bug: Adding an item to a list doesn't seem to respect the position
            // This is used on new content item (+)
            var indexNullAddToEnd = primaryItem.Header.Index == null;
            var willAdd = primaryItem.Header.AddSafe;

            l.A($"will add: {willAdd}; Group.Add:{primaryItem.Header.Add}; EntityId:{primaryItem.Entity.EntityId}");

            var fieldPair = targetIsContentBlock
                ? ViewParts.PickFieldPair(primaryItem.Header.Field!)
                : [primaryItem.Header.Field!];

            var fieldList = workFieldList.New(MyOptions);
            if (willAdd) // this cannot be auto-detected, it must be specified
            {

                // handle edge case on app with empty list, when index=1, but it should be index=0 (indexNullAddToEnd=true have the same effect)
                // fix https://github.com/2sic/2sxc/issues/2943 
                if (!parent.Children(fieldPair.First()).Any() && !targetIsContentBlock)
                    indexNullAddToEnd = true;

                fieldList.AddSlot(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft, indexNullAddToEnd,
                    targetIsContentBlock);
            }
            else
                fieldList.ReplaceSlot(parent, fieldPair, index, ids, block.Context.Publishing.ForceDraft);

        }

        // update-module-title
        var ok = await blockEditor.New().Handle(new(), new(block));
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
    private static int GetIdFromGuidOrError(Dictionary<Guid, int> postSaveIds, Guid guid) =>
        postSaveIds.TryGetValue(guid, out var id)
            ? id
            : throw new("Saved entity not found - not able to update BlockConfiguration");

    private int? FindPresentationItem(Dictionary<Guid, int> postSaveIds,
        IGrouping<string, BundleWithHeader<IEntity>> bundle)
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
}
