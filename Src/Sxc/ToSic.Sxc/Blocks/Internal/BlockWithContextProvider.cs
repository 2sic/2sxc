using ToSic.Lib.Helpers;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This is an important helper.
/// Background: When a block is prepared, we sometimes need the context early on to do security checks.
/// 1. But we cannot use the Block.Context
/// 2. because that again requires the Block first
/// 3. which would cause a StackOverflow because it needs to be created first
/// 4. which in turn requires the context - so it would loop and die
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockWithContextProvider(IContextOfBlock contextOfBlock, Func<IBlock> delayedBlockGen)
{
    public IContextOfBlock ContextOfBlock { get; } = contextOfBlock;
    public IBlock LoadBlock() => _block.Get(delayedBlockGen);
    private readonly GetOnce<IBlock> _block = new();
}