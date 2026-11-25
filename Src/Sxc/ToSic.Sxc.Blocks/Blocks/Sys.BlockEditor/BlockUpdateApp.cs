using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

/// <summary>
/// Trivial action to update the AppId.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdateApp(BlockSavePicker savePicker) : ServiceBase("Sxc.BlkUpdApp", connect: [savePicker]), IWork<(IBlock block, int? appId), bool>
{
    public Task<Package<bool>> Handle(WorkContext mainCtx, Package<(IBlock block, int? appId)> package)
    {
        var (block, appId) = package.Data;
        SetAppId(block, appId);
        return Task.FromResult(new Package<bool>(true));
    }

    /// <summary>
    /// Update AppId this block is referencing
    /// </summary>
    /// <param name="appId"></param>
    private void SetAppId(IBlock block, int? appId)
        => savePicker.PickUpdater(block)
            .SetAppId(block.Context.Module, appId);


}
