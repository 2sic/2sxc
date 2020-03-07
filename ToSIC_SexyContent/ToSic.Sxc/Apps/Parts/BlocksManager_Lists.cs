using ToSic.Eav.Apps.Parts;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Apps
{
    public partial class BlocksManager
    {

        public void AddEmptyItem(BlockConfiguration block, int? sortOrder = null)
            => AddContentAndPresentationEntity(block, 
                sortOrder, 
                null, 
                null);

        public void ChangeOrder(BlockConfiguration block, int sortOrder, int destinationSortOrder)
        {
            var wrapLog = Log.Call($"change order orig:{sortOrder}, dest:{destinationSortOrder}");
            GetEntityListManager(block).ReorderAndSave(sortOrder, destinationSortOrder);
            wrapLog(null);
        }

        public void RemoveFromList(BlockConfiguration block, int sortOrder)
        {
            Log.Add($"remove from list order:{sortOrder}");
            GetEntityListManager(block)
                .RemoveInPair(sortOrder);
        }


        public void UpdateEntityIfChanged(
            BlockConfiguration block, 
            int sortOrder, 
            int? entityId, 
            bool updatePair,
            int? presentationId)
        {
            GetEntityListManager(block).UpdateEntityIfChanged(sortOrder, entityId, updatePair, presentationId);
        }

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
        /// <param name="block"></param>
        /// <param name="sortOrder"></param>
        /// <param name="contentId"></param>
        /// <param name="presentationId"></param>
        public void AddContentAndPresentationEntity(BlockConfiguration block, /*string type,*/ int? sortOrder, int? contentId, int? presentationId)
        {
            GetEntityListManager(block)
                .AddContentAndPresentationEntity(sortOrder, contentId, presentationId);
        }
        

        public bool ReorderAllAndSave(BlockConfiguration block, 
            int[] newSequence)
        {
            return GetEntityListManager(block)
                .ReorderAllAndSave(newSequence);
        }

    }
}
