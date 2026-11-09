using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdateTitle(LazySvc<AppWorkContextService> appWorkCtxSvc, AppWorkChain<WorkBlocks> appBlocks, BlockSavePicker savePicker)
    : ServiceBase("Sxc.BlkEdt", connect: [appWorkCtxSvc, appBlocks, savePicker]), IWork<IBlock, bool>
{
    public Task<Package<bool>> Handle(WorkContext mainCtx, Package<IBlock> package)
    {
        try
        {
            UpdateTitle(package.Data);
            return Task.FromResult(true.ToPackage());
        }
        catch (Exception exception)
        {
            return Task.FromException<Package<bool>>(exception);
        }
    }

    /// <summary>
    /// Update the title.
    /// </summary>
    private void UpdateTitle(IBlock block)
    {
        var l = Log.Fn("update title");

        // check the blockConfiguration as to what should be the module title, then try to set it
        // technically it could have multiple different groups to save in, 
        // ...but for now we'll just update the current modules title
        // note: it also correctly handles published/unpublished, but I'm not sure why :)
        var ctx = appWorkCtxSvc.Value.ContextNew(block.Context.AppReaderRequired);
        var contentGroup = appBlocks
            .New(ctx)
            .GetBlockConfig(block.Configuration.Guid);

        var titleEntity = contentGroup.Header.FirstOrDefault()
                          ?? contentGroup.Content.FirstOrDefault();

        if (titleEntity != null)
            savePicker
                .PickUpdater(block)
                .UpdateTitle(block, titleEntity);

        l.Done();
    }

}