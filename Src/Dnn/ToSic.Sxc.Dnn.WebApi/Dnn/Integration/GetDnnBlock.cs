using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using static ToSic.Sxc.Backend.SxcWebApiConstants;

namespace ToSic.Sxc.Dnn.Integration;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnGetBlock(
    Generator<BlockFromEntity> blockFromEntity,
    Generator<IModuleAndBlockBuilder> moduleAndBlockBuilder)
    : ServiceBase($"{LogName}.GetBlk", connect: [blockFromEntity, moduleAndBlockBuilder])
{
    internal IBlock GetCmsBlock(HttpRequestMessage request)
    {
        var l = Log.Fn<IBlock>(timer: true);
        var moduleInfo = request.FindModuleInfo();

        if (moduleInfo == null)
            return l.ReturnNull("request ModuleInfo not found");

        var block = moduleAndBlockBuilder.New().BuildBlock(moduleInfo, null);

        // check if we need an inner block
        if (!request.Headers.Contains(HeaderContentBlockId))
            return l.Return(block, "normal block, no inner-content");

        // only if it's negative, do we load the inner block
        var blockHeaderId = request.Headers.GetValues(HeaderContentBlockId).FirstOrDefault();
        int.TryParse(blockHeaderId, out var contentBlockId);

        // only if ID is negative, do we load the inner block
        if (contentBlockId >= 0)
            return l.Return(block, "normal block");

        var entBlock = GetBlockOrInnerContentBlock(request, block, contentBlockId);
        return l.Return(entBlock, $"inner content-block {contentBlockId}");
    }

    private IBlock GetBlockOrInnerContentBlock(HttpRequestMessage request, IBlock block, int blockId)
    {
        var l = Log.Fn<IBlock>($"{nameof(blockId)}: {blockId}");

        // If we have a list of inner-blocks (WIP, I believe not implemented) do we go down the list of blocks to find the inner-most one
        if (request.Headers.Contains(HeaderContentBlockList))
        {
            var blockIds = request.Headers
                .GetValues(HeaderContentBlockList)
                .FirstOrDefault()?
                .CsvToArrayWithoutEmpty();
            if (blockIds.SafeAny())
                return l.Return(FindInnerContentParentBlock(block, blockId, blockIds), $"from {HeaderContentBlockList}");
        }

        var entBlock = blockFromEntity.New().Init(block, null, blockId);

        return l.Return(entBlock);
    }

    private IBlock FindInnerContentParentBlock(IBlock parent, int contentBlockId, IReadOnlyCollection<string> blockIds)
    {
        if (blockIds == null || blockIds.Count < 2) return parent;

        foreach (var ids in blockIds) // blockIds is ordered list, from first ancestor till last successor 
        {
            var parentIds = ids.Split(':');
            //var parentAppId = int.Parse(parentIds[0]);
            //var parentContentBlocks = new Guid(parentIds[1]);
            var id = int.Parse(parentIds[0]);
            if (!int.TryParse(parentIds[1], out var cbid) || id == cbid || cbid >= 0) continue;
            if (cbid == contentBlockId) break; // we are done, because block should be parent/ancestor of cbid
            parent = blockFromEntity.New().Init(parent, null, cbid);
        }

        return parent;
    }
}