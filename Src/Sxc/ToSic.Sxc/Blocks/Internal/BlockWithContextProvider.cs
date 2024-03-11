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
///
/// TODO: 2024-03-11 2DM - NOT SURE IF THIS IS STILL RELEVANT!
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockWithContextProvider(IContextOfBlock contextOfBlock, IBlock blockMaybeNull)
{
    public IContextOfBlock ContextOfBlock => contextOfBlock;
    public IBlock LoadBlock() => _blockGetOnce.Get(() => blockMaybeNull);
    private readonly GetOnce<IBlock> _blockGetOnce = new();

}