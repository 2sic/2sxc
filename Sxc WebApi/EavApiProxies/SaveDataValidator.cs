using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    internal class SaveDataValidator: ValidatorBase
    {
        public AllInOne Package;
        internal AppRuntime AppRead;

        public SaveDataValidator(AllInOne package, Log parentLog) : base("Val.Save", parentLog, "start save validator")
        {
            Package = package;
        }

        public void PrepareForEntityChecks(AppRuntime appRead) => AppRead = appRead;

        internal bool ContainsOnlyExpectedNodes(out HttpResponseException preparedException)
        {
            Log.Add("ContainsOnlyExpectedNodes()");
            if (Package.ContentTypes != null)
                Add("package contained content-types, unexpected!");
            if (Package.InputTypes != null)
                Add("package contained input types, unexpected!");
            if (Package.Features != null)
                Add("package contained features, unexpected!");

            // check that items are mostly intact
            if (Package.Items == null || Package.Items.Count == 0)
                Add("package didn't contain items, unexpected!");
            else
            {
                // do various validity tests on items
                VerifyAllGroupAssignmentsValid(Package.Items);
                ValidateEachItemInBundle(Package.Items);
            }

            return BuildTrueIfOk(out preparedException, "ContainsOnlyExpectedNodes() done");
        }

        /// <summary>
        /// Do various validity checks on each item
        /// </summary>
        private void ValidateEachItemInBundle(IList<BundleWithHeader<JsonEntity>> list)
        {
            Log.Add($"ValidateEachItemInBundle({list.Count})");
            foreach (var item in list)
            {
                if (item.Header == null || item.Entity == null)
                    Add($"item {list.IndexOf(item)} header or entity is missing");
                else if(item.Header.Guid != item.Entity.Guid) // check this first (because .Group may not exist)
                {
                    if(item.Header.Group == null)
                        Add($"item {list.IndexOf(item)} has guid missmatch on header/entity, and doesn't have a group");
                    else if (!item.Header.Group.SlotIsEmpty)
                        Add($"item {list.IndexOf(item)} header / entity guid missmatch");
                    // otherwise we're fine
                }
            }
        }

        /// <summary>
        /// ensure all want to save to the same assignment type - either in group or not!
        /// </summary>
        private void VerifyAllGroupAssignmentsValid(IReadOnlyCollection<BundleWithHeader<JsonEntity>> list)
        {
            Log.Add($"VerifyAllGroupAssignmentsValid({list.Count})");
            var groupAssignments = list.Select(i => i.Header.Group).Where(g => g != null).ToList();
            if (groupAssignments.Count == 0)
            {
                Log.Add("none of the items is part of a list/group");
                return;
            }

            if (groupAssignments.Count != list.Count)
                Add($"Items in package with group: {groupAssignments} " +
                    $"- should be 0 or {list.Count} (items in list) " +
                    "- must stop, never expect items to come from different sources");
            else
            {
                var firstInnerContentAppId = groupAssignments.First().ContentBlockAppId;
                if (list.Any(i => i.Header.Group.ContentBlockAppId != firstInnerContentAppId))
                    Add("not all items have the same Group.ContentBlockAppId - this is required when using groups");
            }
        }


        internal bool EntityIsOk(int count, IEntity ent, out HttpResponseException preparedException)
        {
            Log.Add($"EntityIsOk({count})");
            if (ent == null)
            {
                Add($"entity {count} couldn't deserialize");
                return BuildTrueIfOk(out preparedException);
            }

            if (ent.Attributes.Count == 0)
                Add($"entity {count} doesn't have attributes (or they are invalid)");

            var original = AppRead.Entities.Get(ent.EntityId) 
                ?? AppRead.Entities.Get(ent.EntityGuid);

            if (original != null)
            {
                CompareIdentities(ent, count, original);
                CompareAttributes(ent, count, original);
            }

            return BuildTrueIfOk(out preparedException, "EntityIsOk() done");
        }

        private void CompareIdentities(IEntity ent, int count, IEntity original)
        {
            if(original.EntityId != ent.EntityId)
                Add($"entity id missmatch on {count} - {ent.EntityId}/{original.EntityId}");

            if(original.EntityGuid != ent.EntityGuid)
                Add($"entity guid missmatch on {count} - {ent.EntityGuid}/{original.EntityGuid}");
        }

        private void CompareAttributes(IEntity ent, int count, IEntity original)
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
        }
    }
}
