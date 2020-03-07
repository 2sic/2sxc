using ToSic.Eav.Apps.Parts;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
    public partial class BlocksManager
    {

        public void AddEmptyItem(BlockConfiguration block, int? sortOrder = null)
            => AddContentAndPresentationEntity(block, sortOrder, null, null);

        public void ChangeOrder(BlockConfiguration block, int sortOrder, int destinationSortOrder)
        {
            var wrapLog = Log.Call($"change order orig:{sortOrder}, dest:{destinationSortOrder}");
            GetEntityListManager(block).Move(sortOrder, destinationSortOrder);
            wrapLog(null);
        }

        public void RemoveFromList(BlockConfiguration block, int sortOrder) 
            => GetEntityListManager(block).Remove(sortOrder);


        public void UpdateEntityIfChanged(BlockConfiguration block, int sortOrder, 
            int? entityId, bool updatePair, int? presentationId) =>
            GetEntityListManager(block).Replace(sortOrder, entityId, updatePair, presentationId);

        private EntityListManager GetEntityListManager(BlockConfiguration block)
            => new EntityListManager(CmsManager, block.Entity,
                ViewParts.Content,
                ViewParts.Presentation,
                block.VersioningEnabled,
                Log);

        public void ResetBlockEntity(BlockConfiguration block) 
            => block.Entity = CmsManager.Read.Blocks.GetBlockConfig(block.Entity.EntityGuid).Entity;

        /// <summary>
        /// If SortOrder is not specified, adds at the end
        /// </summary>
        public void AddContentAndPresentationEntity(BlockConfiguration block, int? sortOrder, int? contentId, int? presentationId) =>
            GetEntityListManager(block).Add(sortOrder, contentId, presentationId);

        public void ReorderAllAndSave(BlockConfiguration block, int[] newSequence) =>
            GetEntityListManager(block).Reorder(newSequence);
    }
}
