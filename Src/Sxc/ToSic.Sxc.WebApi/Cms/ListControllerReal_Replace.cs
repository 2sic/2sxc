using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class ListControllerReal
    {

        public void Replace(Guid guid, string part, int index, int entityId, bool add = false) => Log.Do($"target:{guid}, part:{part}, index:{index}, id:{entityId}", ()=>
        {
            void InternalSave(VersioningActionInfo _)
            {
                var entity = CmsManagerOfBlock.AppState.List.One(guid);
                if (entity == null) throw new Exception($"Can't find item '{guid}'");

                // Make sure we have the correct casing for the field names
                part = entity.Type[part].Name;

                var forceDraft = Context.Publishing.ForceDraft;
                if (add)
                    CmsManagerOfBlock.Entities.FieldListAdd(entity, new[] { part }, index, new int?[] { entityId }, forceDraft, false);
                else
                    CmsManagerOfBlock.Entities.FieldListReplaceIfModified(entity, new[] { part }, index, new int?[] { entityId },
                        forceDraft);
            }

            // use dnn versioning - this is always part of page
            _versioning.New().DoInsidePublishing(Context, InternalSave);
        });


        internal ReplacementListDto BuildReplaceList(Guid guid, string part, int index, string typeName)
        {
            var wrapLog = Log.Fn<ReplacementListDto>($"{nameof(typeName)}:{typeName}, {nameof(part)}:{part}, {nameof(index)}:{index}");

            var (existingItemsInField, typeNameOfField) = FindItemAndFieldTypeName(guid, part);

            typeName = typeName ?? typeNameOfField;

            // if no type was defined in this set, then return an empty list as there is nothing to choose from
            if (string.IsNullOrEmpty(typeName))
                return wrapLog.ReturnNull("no type name, so no data");

            var ct = Context.AppState.GetContentType(typeName);

            var listTemp = CmsManagerOfBlock.Read.Entities.Get(typeName);

            var results = listTemp.ToDictionary(
                p => p.EntityId,
                p => p.GetBestTitle() ?? "");

            // if list is empty or shorter than index (would happen in an add-to-end-request) return null
            var selectedId = existingItemsInField.Count > index
                ? existingItemsInField[index]?.EntityId
                : null;

            var result = new ReplacementListDto { SelectedId = selectedId, Items = results, ContentTypeName = ct.NameId };
            return wrapLog.Return(result);
        }


        private (List<IEntity> items, string typeName) FindItemAndFieldTypeName(Guid guid, string part)
        {
            var parent = Context.AppState.List.One(guid);
            if (parent == null) throw new Exception($"No item found for {guid}");
            if (!parent.Attributes.ContainsKey(part)) throw new Exception($"Could not find field {part} in item {guid}");
            var itemList = parent.Children(part);

            // find attribute-type-name
            var attribute = parent.Type[part];
            if (attribute == null) throw new Exception($"Attribute definition for '{part}' not found on the item {guid}");
            var typeNameForField = attribute.EntityFieldItemTypePrimary();
            return (itemList, typeNameForField);
        }


    }
}
