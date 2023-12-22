using System;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Blocks;

/// <summary>
/// This is an important helper.
/// Background: When a block is prepared, we sometimes need the context early on to do security checks.
/// 1. But we cannot use the Block.Context
/// 2. because that again requires the Block first
/// 3. which would cause a StackOverflow because it needs to be created first
/// 4. which in turn requires the context - so it would loop and die
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockWithContextProvider
{

    public BlockWithContextProvider(IContextOfBlock contextOfBlock, Func<IBlock> delayedBlockGen)
    {
        _delayedBlockGen = delayedBlockGen;
        ContextOfBlock = contextOfBlock;
    }
    private readonly Func<IBlock> _delayedBlockGen;

    public IContextOfBlock ContextOfBlock { get; }
    public IBlock LoadBlock() => _block.Get(_delayedBlockGen);
    private readonly GetOnce<IBlock> _block = new();
}