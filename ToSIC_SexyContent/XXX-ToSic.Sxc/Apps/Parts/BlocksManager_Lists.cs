using System;
using ToSic.Eav.Apps.Parts.Tools;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using UpdateList = System.Collections.Generic.Dictionary<string, object>;

namespace ToSic.Sxc.Apps
{
    public partial class BlocksManager
    {
        // TODO replace with direct call to Entities.FieldListReplaceIfModified
        public void AddEmptyItem(BlockConfiguration block, int? sortOrder = null)
            => AddContentAndPresentationEntity(block, ViewParts.ContentPair, sortOrder, null, null);

        //public void ChangeOrder(BlockConfiguration block, int sourceIndex, int targetIndex)
        //{
        //    var wrapLog = Log.Call($"change order orig:{sourceIndex}, dest:{targetIndex}");
        //    /*
        //     * Known Issue 2017-08-28:
        //     * Should case DRAFT copy of the BlockConfiguration if versioning is enabled.
        //     */
        //    FieldListUpdate(block, ViewParts.ContentPair, lists => lists.Move(sourceIndex, targetIndex));
        //    wrapLog(null);
        //}

        //public void UpdateEntityIfChanged(BlockConfiguration block, string[] fields, int sortOrder, Tuple<bool, int?>[] values)
        //    => FieldListUpdate(block, fields, lists => lists.Replace(sortOrder, values));

        //public void ResetBlockEntity(BlockConfiguration block) 
        //    => block.Entity = CmsManager.Read.Blocks.GetBlockConfig(block.Entity.EntityGuid).Entity;

        /// <summary>
        /// If SortOrder is not specified, adds at the end
        /// </summary>
        private /*public*/ void AddContentAndPresentationEntity(BlockConfiguration block, string[] fields, int? sortOrder, int? contentId, int? presentationId)
            => FieldListUpdate(block, fields, lists => lists.Add(sortOrder, new[] { contentId, presentationId }));

        //public void ReorderAllAndSave(BlockConfiguration block, int[] newSequence) 
        //    => FieldListUpdate(block, ViewParts.ContentPair, lists => lists.Reorder(newSequence));

        private void FieldListUpdate(BlockConfiguration block, string[] fields, Func<CoupledIdLists, UpdateList> callback)
        {
            AppManager.Entities.FieldListUpdate(block.Entity, fields, block.VersioningEnabled, callback);
            //var lists = new CoupledIdLists(fields.ToDictionary(f => f, f => IdsWithNulls(block.Entity.Children(f))), Log);
            //var values = callback.Invoke(lists);
            //AppManager.Entities.UpdateParts(block.Entity, values, block.VersioningEnabled);
        }
    }
}
