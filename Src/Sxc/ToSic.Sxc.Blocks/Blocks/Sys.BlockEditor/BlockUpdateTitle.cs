using ToSic.Sxc.Blocks.Sys.Work;

namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockUpdateTitle(LazySvc<AppWorkContextService> appWorkCtxSvc, AppWorkChain<WorkBlocks> appBlocks, BlockSavePicker savePicker)
    : ServiceBase("Sxc.BlkEdt", connect: [appWorkCtxSvc, appBlocks, savePicker])
{
    /// <summary>
    /// Update the title.
    /// </summary>
    /// <param name="block1"></param>
    public void UpdateTitle(IBlock block)
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