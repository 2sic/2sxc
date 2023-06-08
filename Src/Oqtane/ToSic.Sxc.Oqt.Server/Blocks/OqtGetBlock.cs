using Oqtane.Repository;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Blocks
{
    /// <summary>
    /// WIP - separating concerns in OqtState to get the block and provide the state...
    /// </summary>
    public class OqtGetBlock: ServiceBase
    {
        public OqtGetBlock(
            LazySvc<IModuleRepository> modRepoLazy,
            RequestHelper requestHelper,
            IContextResolver contextResolverToInit,
            Generator<IContextOfBlock> cntOfBlkGen,
            Generator<BlockFromModule> blkFromModGen,
            Generator<BlockFromEntity> blkFromEntGen
        ) : base($"{OqtConstants.OqtLogPrefix}.GetBlk")
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

        public IContextResolver TryToLoadBlockAndAttachToResolver()
        {
            if (_alreadyTriedToLoad) return _contextResolverToInit;
            _alreadyTriedToLoad = true;

            var block = GetBlockWithContextProvider();
            _contextResolverToInit.AttachBlock(block);
            return _contextResolverToInit;
        }
        private bool _alreadyTriedToLoad;

        private BlockWithContextProvider GetBlockWithContextProvider() => _blockWithContextProvider.Get(InitializeBlock);
        private readonly GetOnce<BlockWithContextProvider> _blockWithContextProvider = new();


        internal BlockWithContextProvider InitializeBlock()
        {
            var wrapLog = Log.Fn<BlockWithContextProvider>();

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            var moduleId = TryGetModuleId();
            if (moduleId == Eav.Constants.NullId)
                return wrapLog.ReturnNull("missing block because ModuleId not found in request");

            var pageId = TryGetPageId();
            if (pageId == Eav.Constants.NullId)
                return wrapLog.ReturnNull("missing block because PageId not found in request");

            var module = _modRepoLazy.Value.GetModule(moduleId);
            var ctx = _cntOfBlkGen.New().Init(pageId, module);
            var block = _blkFromModGen.New().Init(ctx);

            // only if it's negative, do we load the inner block
            var contentBlockId = _requestHelper.GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            if (contentBlockId >= 0)
                return wrapLog.Return(new(ctx, () => block)/* block*/, "found block");

            Log.A($"Inner Content: {contentBlockId}");
            var entityBlock = _blkFromEntGen.New().Init(block, null, contentBlockId);
            return wrapLog.Return(new(entityBlock.Context, () => entityBlock)/* entityBlock*/, "found inner block");
        }

        private int TryGetPageId()
        {
            var wrapLog = Log.Fn<int>();

            var pageId = _requestHelper.TryGetPageId();

            return pageId == Eav.Constants.NullId
                ? wrapLog.Return(Eav.Constants.NullId, "error, pageId not found") 
                : wrapLog.Return(pageId, "ok, found pageId");
        }

        private int TryGetModuleId()
        {
            var wrapLog = Log.Fn<int>();

            var moduleId = _requestHelper.TryGetModuleId();

            return moduleId == Eav.Constants.NullId
                ? wrapLog.Return(Eav.Constants.NullId, "error, moduleId not found")
                : wrapLog.Return(moduleId, "ok, found moduleId");
        }
    }
}
