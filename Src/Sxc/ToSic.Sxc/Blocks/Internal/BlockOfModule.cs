using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// Official constructor, must call Init afterward
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public sealed class BlockFromModule(BlockBase.MyServices services) : BlockBase(services, "CB.Mod")
{
    /// <summary>
    /// Create a module-content block
    /// </summary>
    /// <param name="ctx"></param>
    ///// <param name="overrideParams">optional override parameters</param>
    public BlockFromModule Init(IContextOfBlock ctx)
    {
        var l = Log.Fn<BlockFromModule>(timer: true);
        Init(ctx, ctx.Module.BlockIdentifier);
        IsContentApp = ctx.Module.IsContent;
        CompleteInit(null, ctx.Module.BlockIdentifier, ctx.Module.Id);
        return l.ReturnAsOk(this);
    }

}