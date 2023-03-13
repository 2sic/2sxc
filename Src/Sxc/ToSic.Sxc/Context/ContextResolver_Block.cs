using System;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context
{
    public partial class ContextResolver
    {
        public void AttachBlock(Func<IBlock> getBlock)
        {
            _getBlock = getBlock;
            _block.Reset();
        }
        private Func<IBlock> _getBlock;

        public IBlock BlockOrNull() => _block.Get(() => _getBlock?.Invoke());
        private readonly GetOnce<IBlock> _block = new GetOnce<IBlock>();

        public IBlock BlockRequired() => BlockOrNull() ?? throw new Exception("Block required but missing. It was not attached");

        private IContextOfBlock BlockContext => _blockContext.Get(() => BlockOrNull()?.Context);
        private readonly GetOnce<IContextOfBlock> _blockContext = new GetOnce<IContextOfBlock>();


        public IContextOfBlock BlockContextRequired() => BlockContext ?? throw new Exception("Block context required but not known. It was not attached.");

        public IContextOfBlock BlockContextOrNull() => BlockContext;

    }
}
