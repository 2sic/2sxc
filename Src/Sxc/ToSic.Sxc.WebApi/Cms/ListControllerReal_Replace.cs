using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.WebApi.Cms;

partial class ListControllerReal
{

    public void Replace(Guid guid, string part, int index, int entityId, bool add)
    {
        var isContentPair = ViewParts.ContentLower.EqualsInsensitive(part);

        var l = Log.Fn($"target:{guid}, {nameof(part)}:{part}, {nameof(isContentPair)}: {isContentPair}, {nameof(index)}:{index}, {nameof(entityId)}:{entityId}, {nameof(add)}: {add}");

        void InternalSave(VersioningActionInfo _)
        {
            var entity = AppWorkCtx.AppState.GetDraftOrPublished(guid)
                         ?? throw l.Done( new Exception($"Can't find item '{guid}'"));

            // Make sure we have the correct casing for the field names
            part = entity.Type[part].Name;

            var fList = _workFieldList.New(Context.AppState);

            var forceDraft = Context.Publishing.ForceDraft;
            if (add)
            {
                var fields = isContentPair ? ViewParts.ContentPair : new[] { part };
                var values = isContentPair ? new int?[] { entityId, null } : new int?[] { entityId };
                fList.FieldListAdd(entity, fields, index, values, forceDraft, false);
            }
            else
                fList.FieldListReplaceIfModified(entity, new[] { part }, index, new int?[] { entityId },
                    forceDraft);
        }

        // use dnn versioning - this is always part of page
        _versioning.New().DoInsidePublishing(Context, InternalSave);
        l.Done();
    }


    internal ReplacementListDto GetListToReorder(Guid guid, string part, int index, string typeName)
    {
        var l = Log.Fn<ReplacementListDto>($"{nameof(typeName)}:{typeName}, {nameof(part)}:{part}, {nameof(index)}:{index}");

        var (existingItemsInField, typeNameOfField) = FindItemAndFieldTypeName(guid, part);

        typeName = typeName ?? typeNameOfField;

        // if no type was defined in this set, then return an empty list as there is nothing to choose from
        if (string.IsNullOrEmpty(typeName))
            return l.ReturnNull("no type name, so no data");

        var ct = Context.AppState.GetContentType(typeName);

        var listTemp = _workEntities.New(Context.AppState).Get(typeName).ToList();

        var results = listTemp.Select(Context.AppState.GetDraftOrKeep).ToDictionary(
            p => p.EntityId,
            p => p.GetBestTitle() ?? "");

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
        var parent = Context.AppState.GetDraftOrPublished(guid);
        if (parent == null) throw l.Done(new Exception($"No item found for {guid}"));
        if (!parent.Attributes.ContainsKey(part)) throw l.Done(new Exception($"Could not find field {part} in item {guid}"));
        var itemList = parent.Children(part).Select(Context.AppState.GetDraftOrKeep).ToList();

        // find attribute-type-name
        var attribute = parent.Type[part];
        if (attribute == null) throw l.Done(new Exception($"Attribute definition for '{part}' not found on the item {guid}"));
        var typeNameForField = attribute.EntityFieldItemTypePrimary();
        return l.ReturnAsOk((itemList, typeNameForField));
    }


}