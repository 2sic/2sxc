using ToSic.Eav.Data.ListSlots.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Blocks.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlocksAddSlot(AppWorkChain<WorkListSlots> workSlots)
    : ServiceWithSetup<IAppWorkContext>("AWk.EntCre", connect: [workSlots])
{
    
    public void AddEmptyItem(BlockConfiguration block, int? index, bool forceDraft)
    {
        var slotWork = workSlots.New(MyOptions);
        var entity = ((ICanBeEntity)block).Entity;

        // Check how many rows to add - depending on if (+) was used the first time or later
        // hitting (+) if the list is empty add two demo items (because we already see one demo item)
        var existingRows = entity.Children(ViewParts.Content);
        var finalIndex = existingRows.Any() ? index ?? 0 : 1;
        slotWork.AddSlot(target: entity, fields: ViewParts.ContentPair, index: finalIndex, values: [null, null], asDraft: forceDraft, forceAddToEnd: false, padWithNulls: true);
    }
}
