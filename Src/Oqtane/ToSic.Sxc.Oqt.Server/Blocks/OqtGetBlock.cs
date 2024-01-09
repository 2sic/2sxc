using Oqtane.Repository;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.WebApi.Infrastructure;

namespace ToSic.Sxc.Oqt.Server.Blocks;

/// <summary>
/// WIP - separating concerns in OqtState to get the block and provide the state...
/// </summary>
internal class OqtGetBlock: ServiceBase, IWebApiContextBuilder
{
    public OqtGetBlock(
        LazySvc<IModuleRepository> modRepoLazy,
        RequestHelper requestHelper,
        IContextResolver contextResolverToInit,
        Generator<IContextOfBlock> cntOfBlkGen,
        Generator<BlockFromModule> blkFromModGen,
        Generator<BlockFromEntity> blkFromEntGen
    ) : base("Sxc.GetBlk")
    {
        ConnectServices(
            _modRepoLazy = modRepoLazy,
            _requestHelper = requestHelper,
            _contextResolverToInit = contextResolverToInit,
            _cntOfBlkGen = cntOfBlkGen,
            _blkFromModGen = blkFromModGen,
            _blkFromEntGen = blkFromEntGen
        );
    }

    private readonly LazySvc<IModuleRepository> _modRepoLazy;
    private readonly RequestHelper _requestHelper;
    private readonly IContextResolver _contextResolverToInit;
    private readonly Generator<IContextOfBlock> _cntOfBlkGen;
    private readonly Generator<BlockFromModule> _blkFromModGen;
    private readonly Generator<BlockFromEntity> _blkFromEntGen;

    public IContextResolver PrepareContextResolverForApiRequest()
    {
        if (_alreadyTriedToLoad) return _contextResolverToInit;
        _alreadyTriedToLoad = true;

        var block = InitializeBlock();
        _contextResolverToInit.AttachBlock(block);
        return _contextResolverToInit;
    }
    private bool _alreadyTriedToLoad;


    private BlockWithContextProvider InitializeBlock()
    {
        var wrapLog = Log.Fn<BlockWithContextProvider>();

        // WebAPI calls can contain the original parameters that made the page, so that views can respect that
        var moduleId = TryGetId(ContextConstants.ModuleIdKey);
        if (moduleId == Eav.Constants.NullId)
            return wrapLog.ReturnNull("missing block because ModuleId not found in request");

        var pageId = TryGetId(ContextConstants.PageIdKey);
        if (pageId == Eav.Constants.NullId)
            return wrapLog.ReturnNull("missing block because PageId not found in request");

        var module = _modRepoLazy.Value.GetModule(moduleId);
        var ctx = _cntOfBlkGen.New().Init(pageId, module);
        var block = _blkFromModGen.New().Init(ctx);

        // only if it's negative, do we load the inner block
        var contentBlockId = _requestHelper.GetTypedHeader(Sxc.WebApi.SxcWebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
        if (contentBlockId >= 0)
            return wrapLog.Return(new(ctx, () => block), "found block");

        Log.A($"Inner Content: {contentBlockId}");
        var entityBlock = _blkFromEntGen.New().Init(block, null, contentBlockId);
        return wrapLog.Return(new(entityBlock.Context, () => entityBlock), "found inner block");
    }

    private int TryGetId(string key)
    {
        var l = Log.Fn<int>(key);
        var id = _requestHelper.TryGetId(key);
        return l.Return(id, id == Eav.Constants.NullId ? "not found" : $"found {id}");
    }
}