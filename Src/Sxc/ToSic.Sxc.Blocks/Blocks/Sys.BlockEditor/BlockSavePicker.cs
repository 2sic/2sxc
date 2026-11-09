using ToSic.Sxc.Integration.Modules;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

/// <summary>
/// Options must contain the responsible service to do the final save operation.
/// </summary>
/// <param name="Block"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockSavePicker(Generator<BlockUpdaterEntity, IBlock> entityUpdater, LazySvc<IPlatformModuleUpdater> blockUpdater)
{
    /// <summary>
    /// Helper to pick the right updater for the task, depending on inner-content (entity) or outer-content (module)
    /// </summary>
    /// <returns></returns>
    internal IPlatformModuleUpdater PickUpdater(IBlock block) =>
        block.IsInnerBlock switch
        {
            false => blockUpdater.Value,
            true => entityUpdater.New(block),
        };
}
