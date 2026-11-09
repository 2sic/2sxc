namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

/// <summary>
/// Trivial action to update the AppId.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdateApp(BlockSavePicker savePicker) : ServiceBase("Sxc.BlkUpdApp", connect: [savePicker])
{
    /// <summary>
    /// Update AppId this block is referencing
    /// </summary>
    /// <param name="appId"></param>
    public void SetAppId(IBlock block, int? appId)
        => savePicker.PickUpdater(block)
            .SetAppId(block.Context.Module, appId);

}
