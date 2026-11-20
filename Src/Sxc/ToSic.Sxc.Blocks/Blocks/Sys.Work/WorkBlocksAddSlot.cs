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
        var rowsToAdd = existingRows.Any() ? 1 : 2; // if no rows, add 2, otherwise add 1
        slotWork
            .FieldListUpdate(
                entity,
                ViewParts.ContentPair,
                forceDraft,
                lists =>
                {
                    // Add as many rows as rowsToAdd
                    for (var i = 0; i < rowsToAdd; i++)
                        lists.Add(index, [null, null]);

                    return lists.Lists;
                });
    }
}
