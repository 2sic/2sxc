using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Parts.Tools;
using ToSic.Eav.Data;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using UpdateList = System.Collections.Generic.Dictionary<string, object>;

namespace ToSic.Sxc.Apps
{
    public partial class BlocksManager
    {


        public void AddEmptyItem(BlockConfiguration block, int? sortOrder = null)
            => AddContentAndPresentationEntity(block, ViewParts.ContentPair, sortOrder, null, null);

        public void ChangeOrder(BlockConfiguration block, int sourceIndex, int targetIndex)
        {
            var wrapLog = Log.Call($"change order orig:{sourceIndex}, dest:{targetIndex}");
            /*
             * Known Issue 2017-08-28:
             * Should case DRAFT copy of the BlockConfiguration if versioning is enabled.
             */
            DoAndUpdate(block, ViewParts.ContentPair, lists => lists.Move(sourceIndex, targetIndex));
            wrapLog(null);
        }

        public void RemoveFromList(BlockConfiguration block, int sortOrder)
            => DoAndUpdate(block, ViewParts.ContentPair, lists => lists.Remove(sortOrder));

        public void UpdateEntityIfChanged(BlockConfiguration block, string[] fields, int sortOrder, Tuple<bool, int?>[] values)
            => DoAndUpdate(block, fields, lists => lists.Replace(sortOrder, values));

        private static List<int?> IdsWithNulls(IEnumerable<IEntity> list)
            => list.Select(p => p?.EntityId).ToList();

        public void ResetBlockEntity(BlockConfiguration block) 
            => block.Entity = CmsManager.Read.Blocks.GetBlockConfig(block.Entity.EntityGuid).Entity;

        /// <summary>
        /// If SortOrder is not specified, adds at the end
        /// </summary>
        public void AddContentAndPresentationEntity(BlockConfiguration block, string[] fields, int? sortOrder, int? contentId, int? presentationId)
            => DoAndUpdate(block, fields, lists => lists.Add(sortOrder, new[] { contentId, presentationId }));

        public void ReorderAllAndSave(BlockConfiguration block, int[] newSequence) 
            => DoAndUpdate(block, ViewParts.ContentPair, lists => lists.Reorder(newSequence));

        private void DoAndUpdate(BlockConfiguration block, string[] fieldPair, Func<CoupledIdLists, UpdateList> callback)
        {
            var lists = new CoupledIdLists(
                fieldPair.ToDictionary(f => f, f => IdsWithNulls(block.Entity.Children(f))), Log);
            var values = callback.Invoke(lists);
            AppManager.Entities.UpdateParts(block.Entity, values, block.VersioningEnabled);
        }

    }
}
