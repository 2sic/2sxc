using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

partial class ListControllerReal
{

    public void Replace(Guid guid, string part, int index, int entityId, bool add)
    {
        var isContentPair = ViewParts.ContentLower.EqualsInsensitive(part);

        var l = Log.Fn($"target:{guid}, {nameof(part)}:{part}, {nameof(isContentPair)}: {isContentPair}, {nameof(index)}:{index}, {nameof(entityId)}:{entityId}, {nameof(add)}: {add}");

        // use dnn versioning - this is always part of page
        publishing.New().DoInsidePublishing(Context, InternalSave);
        l.Done();
        return;

        void InternalSave(VersioningActionInfo _)
        {
            var entity = Context.AppReaderRequired.GetDraftOrPublished(guid)
                         ?? throw l.Done( new Exception($"Can't find item '{guid}'"));

            // Make sure we have the correct casing for the field names
            part = entity.Type[part]!.Name;

            var fList = workFieldList.New(Context.AppReaderRequired);

            var forceDraft = Context.Publishing.ForceDraft;
            if (add)
            {
                var fields = isContentPair ? ViewParts.ContentPair : [part];
                var values = isContentPair ? [entityId, null] : new int?[] { entityId };
                fList.FieldListAdd(entity, fields, index, values, forceDraft, false);
            }
            else
                fList.FieldListReplaceIfModified(entity, [part], index, [entityId],
                    forceDraft);
        }
    }


    internal ReplacementListDto? GetListToReorder(Guid guid, string part, int index, string? typeName)
    {
        var l = Log.Fn<ReplacementListDto>($"{nameof(typeName)}:{typeName}, {nameof(part)}:{part}, {nameof(index)}:{index}");

        var (existingItemsInField, typeNameOfField) = FindItemAndFieldTypeName(guid, part);

        typeName ??= typeNameOfField;

        // if no type was defined in this set, then return an empty list as there is nothing to choose from
        if (string.IsNullOrEmpty(typeName))
            return l.ReturnNull("no type name, so no data");

        var ct = Context.AppReaderRequired.GetContentType(typeName);

        var listTemp = workEntities.New(Context.AppReaderRequired)
            .Get(typeName)
            .ToList();

        var preferDraft = listTemp
            .Select(Context.AppReaderRequired.GetDraftOrKeep)
            .GroupBy(e => e!.EntityId)
            .Select(g => g.OrderBy(e => e!.RepositoryId).Last())
            .ToList();

        var results = preferDraft.ToDictionary(
            p => p!.EntityId,
            p => p!.GetBestTitle() ?? ""
        );

        // if list is empty or shorter than index (would happen in an add-to-end-request) return null
        var selectedId = existingItemsInField.Count > index
            ? existingItemsInField[index]?.EntityId
            : null;

        var result = new ReplacementListDto { SelectedId = selectedId, Items = results, ContentTypeName = ct.NameId };
        return l.Return(result);
    }


    private (List<IEntity> items, string typeName) FindItemAndFieldTypeName(Guid guid, string part)
    {
        var l = Log.Fn<(List<IEntity>, string)>($"guid:{guid},part:{part}");
        var parent = Context.AppReaderRequired.GetDraftOrPublished(guid);
        if (parent == null)
            throw l.Done(new Exception($"No item found for {guid}"));
        if (!parent.Attributes.ContainsKey(part))
            throw l.Done(new Exception($"Could not find field {part} in item {guid}"));
        var itemList = parent
            .Children(part)
            .Select(Context.AppReaderRequired.GetDraftOrKeep)
            .Where(e => e != null)
            .Cast<IEntity>()
            .ToList();

        // find attribute-type-name
        var attribute = parent.Type[part];
        if (attribute == null)
            throw l.Done(new Exception($"Attribute definition for '{part}' not found on the item {guid}"));
        var typeNameForField = attribute.EntityFieldItemTypePrimary();
        return l.ReturnAsOk((itemList, typeNameForField));
    }


}