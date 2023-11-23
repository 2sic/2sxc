using System;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    partial class ContextResolver
    {
        public void AttachBlock(BlockWithContextProvider blockWithContextProvider)
        {
            _blcCtx = blockWithContextProvider;
            _block.Reset();
            _blockContext.Reset();
        }
        private BlockWithContextProvider _blcCtx;

        public IBlock BlockOrNull() => _block.Get(() => _blcCtx?.LoadBlock());
        private readonly GetOnce<IBlock> _block = new();

        public IBlock BlockRequired() => BlockOrNull() ?? throw new Exception("Block required but missing. It was not attached");

        public IContextOfBlock BlockContextRequired() => BlockContextOrNull() ?? throw new Exception("Block context required but not known. It was not attached.");

        public IContextOfBlock BlockContextOrNull() => _blockContext.Get(() => _blcCtx?.ContextOfBlock);
        private readonly GetOnce<IContextOfBlock> _blockContext = new();

    }
}
