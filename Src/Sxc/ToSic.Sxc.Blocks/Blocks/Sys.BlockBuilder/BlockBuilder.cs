namespace ToSic.Sxc.Blocks.Sys.BlockBuilder;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class BlockBuilder(Generator<BlockOfModule> blockGenerator, IEnvModuleBuilder envModBuilder)
    : ServiceBase("Sxc.BnMBld", connect: [blockGenerator, envModBuilder]), IBlockBuilder
{
    /// <inheritdoc/>
    public IBlock BuildBlock(int pageId, int moduleId)
    {
        var l = Log.Fn<IBlock>($"{pageId}, {moduleId}");
        
        // Find module definition
        var module = envModBuilder.GetModule(pageId, moduleId);

        // Get context of module and block
        var ctx = envModBuilder.GetContextOfBlock(module, pageId);

        // build the block
        var block = blockGenerator.New().GetBlockOfModule(ctx);

        return l.ReturnAsOk(block);
    }
}

