using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using static ToSic.Sxc.Backend.SxcWebApiConstants;

namespace ToSic.Sxc.Dnn.Integration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnGetBlock(
    Generator<BlockOfEntity> blockFromEntity,
    Generator<IModuleAndBlockBuilder> moduleAndBlockBuilder)
    : ServiceBase($"{LogName}.GetBlk", connect: [blockFromEntity, moduleAndBlockBuilder])
{
    /// <summary>
    /// Get the block specs using the DNN Module Info headers and optionally any content-block headers in the request.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal IBlock? TryGetBlockSpecsUsingModuleAndContentBlockHeaders(HttpRequestMessage request)
    {
        var l = Log.Fn<IBlock>(timer: true);

        // Check if DNN reports a module info for this request, if not exit early
        if (request.FindModuleInfo() is not { } moduleInfo)
            return l.ReturnNull("request ModuleInfo not found");

        // Build the initial / outermost block
        var block = moduleAndBlockBuilder.New().BuildBlock(moduleInfo, null);

        // check if we need an inner block
        if (!request.Headers.Contains(HeaderContentBlockId))
            return l.Return(block, "normal block, no inner-content");

        // See if we have a useful content-block ID in the request header, if not exit early
        // only if ID is negative, do we load the inner block
        var innerContentBlockIdString = request.Headers.GetValues(HeaderContentBlockId).FirstOrDefault();
        int.TryParse(innerContentBlockIdString, out var maybeInnerBlockId);
        if (maybeInnerBlockId >= 0)
            return l.Return(block, "normal block");

        // ID was negative, so it must be an inner-content block; retrieve it now.
        var entBlock = GetBlockOrInnerContentBlock(request, block, maybeInnerBlockId);
        return l.Return(entBlock, $"inner content-block {maybeInnerBlockId}");
    }


    private IBlock GetBlockOrInnerContentBlock(HttpRequestMessage request, IBlock block, int innerBlockId)
    {
        var l = Log.Fn<IBlock>($"{nameof(innerBlockId)}: {innerBlockId}");

        // If we have a list of inner-blocks (WIP, I believe not implemented, probably an early partial work to get this to work)
        // we go down the list of blocks to find the innermost one
        if (request.Headers.Contains(HeaderContentBlockList))
        {
            // Find the http header with the list of inner-blocks, and split it into an array
            var blockIds = request.Headers
                .GetValues(HeaderContentBlockList)
                .FirstOrDefault()?
                .CsvToArrayWithoutEmpty();
            if (blockIds.SafeAny())
                return l.Return(FindDeepInnerContentBlock(block, innerBlockId, blockIds), $"from {HeaderContentBlockList}");
        }

        // Get the inner-content-block
        var entBlock = blockFromEntity.New().GetBlockOfEntity(block, null, innerBlockId);

        return l.Return(entBlock);
    }

    /// <summary>
    /// NOTE: PROBABLY NOT IMPLEMENTED (according to notes in the caller)
    /// Would retrieve the final content-block if we had a list of blocks in blocks, but it's probably not implemented.
    /// </summary>
    /// <param name="rootBlock"></param>
    /// <param name="contentBlockId"></param>
    /// <param name="blockIds"></param>
    /// <returns></returns>
    private IBlock FindDeepInnerContentBlock(IBlock rootBlock, int contentBlockId, IReadOnlyCollection<string> blockIds)
    {
        // If no IDs or less than 2 (so only 0, 1, but no inner-content) exit early.
        if (blockIds == null || blockIds.Count < 2)
            return rootBlock;

        var currentBlock = rootBlock;

        // blockIds is ordered list, from first ancestor till last successor 
        foreach (var ids in blockIds)
        {
            var parentIds = ids.Split(':');
            var id = int.Parse(parentIds[0]);
            if (!int.TryParse(parentIds[1], out var childId) || id == childId || childId >= 0)
                continue;
            if (childId == contentBlockId)
                return currentBlock; // we are done, because block should be parent/ancestor of contentBlockId
            currentBlock = blockFromEntity.New().GetBlockOfEntity(currentBlock, null, childId);
        }

        return currentBlock;
    }
}