using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// This is an important helper.
/// Background: When a block is prepared, we sometimes need the context early on to do security checks.
/// 1. But we cannot use the Block.Context
/// 2. because that again requires the Block first
/// 3. which would cause a StackOverflow because it needs to be created first
/// 4. which in turn requires the context - so it would loop and die
///
/// TODO: 2024-03-11 2DM - NOT SURE IF THIS IS STILL RELEVANT!
/// Refactored it to simplify, because of issue https://github.com/2sic/2sxc/issues/3299
/// If all ok ca. 2024-06, should be fixed, so it doesn't need this helper, since the block always has the context
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockWithContextProvider(IContextOfBlock contextOfBlock, IBlock block)
{
    public BlockWithContextProvider(IBlock block): this (block.Context, block) { }

    public IContextOfBlock ContextOfBlock => contextOfBlock;
    public IBlock LoadBlock() => block;

}