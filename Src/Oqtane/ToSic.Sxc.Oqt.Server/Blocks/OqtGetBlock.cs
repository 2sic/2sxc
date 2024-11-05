using Oqtane.Repository;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using static ToSic.Sxc.Backend.SxcWebApiConstants;

namespace ToSic.Sxc.Oqt.Server.Blocks;

/// <summary>
/// WIP - separating concerns in OqtState to get the block and provide the state...
/// </summary>
internal class OqtGetBlock(
    LazySvc<IModuleRepository> modRepoLazy,
    RequestHelper requestHelper,
    ISxcContextResolver contextResolverToInit,
    Generator<IContextOfBlock> cntOfBlkGen,
    Generator<BlockFromModule> blkFromModGen,
    Generator<BlockFromEntity> blkFromEntGen)
    : ServiceBase("Sxc.GetBlk",
            connect: [modRepoLazy, requestHelper, contextResolverToInit, cntOfBlkGen, blkFromModGen, blkFromEntGen]),
        IWebApiContextBuilder
{
    public ISxcContextResolver PrepareContextResolverForApiRequest()
    {
        if (_alreadyTriedToLoad) return contextResolverToInit;
        _alreadyTriedToLoad = true;

        var block = InitializeBlock();
        contextResolverToInit.AttachBlock(block);
        return contextResolverToInit;
    }
    private bool _alreadyTriedToLoad;


    private IBlock InitializeBlock()
    {
        var l = Log.Fn<IBlock>();

        // WebAPI calls can contain the original parameters that made the page, so that views can respect that
        var moduleId = TryGetId(ContextConstants.ModuleIdKey);
        if (moduleId == Eav.Constants.NullId)
            return l.ReturnNull("missing block because ModuleId not found in request");

        var pageId = TryGetId(ContextConstants.PageIdKey);
        if (pageId == Eav.Constants.NullId)
            return l.ReturnNull("missing block because PageId not found in request");

        var module = modRepoLazy.Value.GetModule(moduleId);
        var ctx = cntOfBlkGen.New().Init(pageId, module);
        var block = blkFromModGen.New().Init(ctx);

        // only if it's negative, do we load the inner block
        var contentBlockId = requestHelper.GetTypedHeader(HeaderContentBlockId, 0); // this can be negative, so use 0
        if (contentBlockId >= 0)
            return l.Return(block, "found block");

        l.A($"Inner Content: {contentBlockId}");
        var entityBlock = blkFromEntGen.New().Init(block, null, contentBlockId);
        return l.Return(entityBlock, $"inner block {contentBlockId}");
    }

    private int TryGetId(string key)
    {
        var l = Log.Fn<int>(key);
        var id = requestHelper.TryGetId(key);
        return l.Return(id, id == Eav.Constants.NullId ? "not found" : $"found {id}");
    }
}