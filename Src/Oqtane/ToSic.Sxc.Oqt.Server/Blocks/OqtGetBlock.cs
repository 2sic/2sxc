using Oqtane.Repository;
using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
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
    public class OqtGetBlock: HasLog<OqtGetBlock>
    {
        public OqtGetBlock(
            Lazy<IModuleRepository> modRepoLazy,
            RequestHelper requestHelper,
            IContextResolver contextResolverToInit,
            Generator<IContextOfBlock> cntOfBlkGen,
            Generator<BlockFromModule> blkFromModGen,
            Generator<BlockFromEntity> blkFromEntGen
        ) : base($"{OqtConstants.OqtLogPrefix}.GetBlk")
        {
            _modRepoLazy = modRepoLazy;
            this.requestHelper = requestHelper;
            _contextResolverToInit = contextResolverToInit;
            _cntOfBlkGen = cntOfBlkGen;
            _blkFromModGen = blkFromModGen;
            _blkFromEntGen = blkFromEntGen;
        }

        private readonly Lazy<IModuleRepository> _modRepoLazy;
        private readonly RequestHelper requestHelper;
        private readonly IContextResolver _contextResolverToInit;
        private readonly Generator<IContextOfBlock> _cntOfBlkGen;
        private readonly Generator<BlockFromModule> _blkFromModGen;
        private readonly Generator<BlockFromEntity> _blkFromEntGen;

        public IContextResolver TryToLoadBlockAndAttachToResolver()
        {
            if (alreadyTriedToLoad) return _contextResolverToInit;
            alreadyTriedToLoad = true;

            var block = GetBlock();
            _contextResolverToInit.AttachRealBlock(() => block);
            return _contextResolverToInit;
        }

        private bool alreadyTriedToLoad;


        private IBlock GetBlock()
        {
            if (_block != null || _triedToGetBlock) return _block;
            _block = InitializeBlock();
            _triedToGetBlock = true;
            return _block;
        }
        private IBlock _block;
        private bool _triedToGetBlock;


        internal IBlock InitializeBlock()
        {
            var wrapLog = Log.Call<IBlock>();

            // WebAPI calls can contain the original parameters that made the page, so that views can respect that
            var moduleId = TryGetModuleId();
            if (moduleId == Eav.Constants.NullId)
                return wrapLog("missing block because ModuleId not found in request", null);

            var pageId = TryGetPageId();
            if (pageId == Eav.Constants.NullId)
                return wrapLog("missing block because PageId not found in request", null);

            var module = _modRepoLazy.Value.GetModule(moduleId);
            var ctx = _cntOfBlkGen.New.Init(pageId, module, Log);
            var block = _blkFromModGen.New.Init(ctx, Log);

            // only if it's negative, do we load the inner block
            var contentBlockId = requestHelper.GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderContentBlockId, 0); // this can be negative, so use 0
            if (contentBlockId >= 0) return wrapLog("found block", block);

            Log.Add($"Inner Content: {contentBlockId}");
            var entityBlock = _blkFromEntGen.New.Init(block, contentBlockId, Log);
            return wrapLog("found inner block", entityBlock);
        }

        private int TryGetPageId()
        {
            var wrapLog = Log.Call<int>();

            var pageId = requestHelper.TryGetPageId();

            return pageId == Eav.Constants.NullId
                ? wrapLog("error, pageId not found", Eav.Constants.NullId) 
                : wrapLog("ok, found pageId", pageId);
        }

        private int TryGetModuleId()
        {
            var wrapLog = Log.Call<int>();

            var moduleId = requestHelper.TryGetModuleId();

            return moduleId == Eav.Constants.NullId
                ? wrapLog("error, moduleId not found", Eav.Constants.NullId)
                : wrapLog("ok, found moduleId", moduleId);
        }
    }
}
