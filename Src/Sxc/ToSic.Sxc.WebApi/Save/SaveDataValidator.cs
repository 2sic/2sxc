using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Lib.Logging;
using ToSic.Eav.Metadata;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Validation;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.Save
{
    internal class SaveDataValidator: ValidatorBase
    {
        public EditDto Package;
        internal AppRuntime AppRead;

        public SaveDataValidator(EditDto package, ILog parentLog = null) : base(parentLog, "Val.Save")
        {
            Package = package;
        }

        public void PrepareForEntityChecks(AppRuntime appRead) => AppRead = appRead;

        /// <summary>
        /// The package format for loading and saving are the same, but we want to make sure
        /// that the save package doesn't contain unexpected trash (which would indicate the UI was broken)
        /// or that invalid combinations get back here
        /// </summary>
        /// <returns></returns>
        internal HttpExceptionAbstraction ContainsOnlyExpectedNodes() => Log.Func(() =>
        {
            if (Package.ContentTypes != null) Add("package contained content-types, unexpected!");
            if (Package.InputTypes != null) Add("package contained input types, unexpected!");
            if (Package.Features != null) Add("package contained features, unexpected!");

            // check that items are mostly intact
            if (Package.Items == null || Package.Items.Count == 0)
                Add("package didn't contain items, unexpected!");
            else
            {
                // do various validity tests on items
                VerifyAllGroupAssignmentsValid(Package.Items);
                ValidateEachItemInBundle(Package.Items);
            }

            BuildExceptionIfHasIssues(out var preparedException, "ContainsOnlyExpectedNodes() done");
            return preparedException;
        });

        /// <summary>
        /// Do various validity checks on each item
        /// </summary>
        private void ValidateEachItemInBundle(IList<BundleWithHeader<JsonEntity>> list) => Log.Do($"{list.Count}", () =>
        {
            foreach (var item in list)
            {
                if (item.Header == null || item.Entity == null)
                    Add($"item {list.IndexOf(item)} header or entity is missing");
                else if (item.Header.Guid != item.Entity.Guid) // check this first (because .Group may not exist)
                {
                    if (!item.Header.IsContentBlockMode)
                        Add(
                            $"item {list.IndexOf(item)} has guid mismatch on header/entity, and doesn't have a group");
                    else if (!item.Header.IsEmpty)
                        Add($"item {list.IndexOf(item)} header / entity guid miss match");
                    // otherwise we're fine
                }
            }
        });

        /// <summary>
        /// ensure all want to save to the same assignment type - either in group or not!
        /// </summary>
        private void VerifyAllGroupAssignmentsValid(IReadOnlyCollection<BundleWithHeader<JsonEntity>> list)
            => Log.Do($"{list.Count}", () =>
            {
                var groupAssignments = list.Select(i => i.Header.ContentBlockAppId).Where(g => g != null).ToList();
                if (groupAssignments.Count == 0)
                    return "none of the items is part of a list/group";

                if (groupAssignments.Count != list.Count)
                    Add($"Items in package with group: {groupAssignments} " +
                        $"- should be 0 or {list.Count} (items in list) " +
                        "- must stop, never expect items to come from different sources");
                else
                {
                    var firstInnerContentAppId = groupAssignments.First();
                    if (list.Any(i => i.Header.ContentBlockAppId != firstInnerContentAppId))
                        Add("not all items have the same Group.ContentBlockAppId - this is required when using groups");
                }

                return "done";
            });


        internal HttpExceptionAbstraction EntityIsOk(int count, IEntity newEntity) => Log.Func(() =>
        {
            if (newEntity == null)
            {
                Add($"entity {count} couldn't deserialize");
                BuildExceptionIfHasIssues(out var preparedException);
                return (preparedException, "newEntity is null");
            }

            // New #2595 allow saving empty metadata decorator entities
            if (newEntity.Attributes.Count == 0 && !newEntity.Type.Metadata.HasType(Decorators.SaveEmptyDecoratorId))
                Add($"entity {count} doesn't have attributes (or they are invalid)");

            BuildExceptionIfHasIssues(out var preparedException2, "EntityIsOk() done");
            return (preparedException2, "second test");
        });

        internal (int? ResetId, HttpExceptionAbstraction Exception)
            IfUpdateValidateAndCorrectIds(int count, IEntity newEntity) => Log.Func(l =>
        {
            var previousEntity = AppRead.Entities.Get(newEntity.EntityId)
                                 ?? AppRead.Entities.Get(newEntity.EntityGuid);

            int? resetId = default;
            if (previousEntity == null)
                return ((null, null), "no previous entity found");


            l.A("found previous entity, will check types/ids/attributes");
            CompareTypes(count, previousEntity, newEntity);

            // for saving, ensure we are using the DB entity-ID 
            if (newEntity.EntityId == 0)
            {
                l.A("found existing entity - will set the ID to that to overwrite");
                resetId = previousEntity.EntityId;
                //newEntity.ResetEntityId(previousEntity.EntityId);
            }

            CompareIdentities(count, previousEntity, newEntity);
            CompareAttributes(count, previousEntity, newEntity);

            BuildExceptionIfHasIssues(out var exception, "EntityIsOk() done");

            return ((resetId, exception), "ok");
        });


        private void CompareTypes(int count, IEntityLight originalEntity, IEntityLight newEntity) =>
            Log.Do($"ids:{newEntity.Type.NameId}/{originalEntity.Type.NameId}", () =>
            {
                if (originalEntity.Type.NameId != newEntity.Type.NameId)
                    Add($"entity type mismatch on {count}");
            });

        private void CompareIdentities(int count, IEntityLight originalEntity, IEntityLight newEntity) =>
            Log.Do($"ids:{newEntity.EntityId}/{originalEntity.EntityId}", () =>
            {
                if (originalEntity.EntityId != newEntity.EntityId)
                    Add($"entity ID mismatch on {count} - {newEntity.EntityId}/{originalEntity.EntityId}");

                Log.A($"Guids:{newEntity.EntityGuid}/{originalEntity.EntityGuid}");
                if (originalEntity.EntityGuid != newEntity.EntityGuid)
                    Add($"entity GUID mismatch on {count} - {newEntity.EntityGuid}/{originalEntity.EntityGuid}");
            });

        private void CompareAttributes(int count, IEntity original, IEntity ent) => Log.Do(() =>
        {
            if (original.Attributes.Count != ent.Attributes.Count)
                Add($"entity {count} has different amount " +
                    $"of attributes {ent.Attributes.Count} " +
                    $"than the original {original.Attributes.Count}");
            else
                foreach (var origAttr in original.Attributes)
                {
                    var newAttr = ent.Attributes.FirstOrDefault(a => a.Key == origAttr.Key);
                    if (newAttr.Equals(default(KeyValuePair<string, IAttribute>)))
                        Add($"attribute {origAttr.Key} not found in save");
                    else if (origAttr.Value.Type != newAttr.Value.Type)
                        Add($"found different type on attribute {origAttr.Key} " +
                            $"- '{origAttr.Value.Type}'/'{newAttr.Value.Type}'");
                }
        });
    }
}
