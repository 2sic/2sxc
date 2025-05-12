using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// Official constructor, must call Init afterward
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class BlockOfModule(BlockServices services) : BlockOfBase(services, "CB.Mod")
{
    /// <summary>
    /// Create a module-content block
    /// </summary>
    /// <param name="ctx"></param>
    ///// <param name="overrideParams">optional override parameters</param>
    public BlockOfModule Init(IContextOfBlock ctx)
    {
        var l = Log.Fn<BlockOfModule>(timer: true);
        Init(ctx, ctx.Module.BlockIdentifier);
        IsContentApp = ctx.Module.IsContent;
        CompleteInit(null, ctx.Module.BlockIdentifier, ctx.Module.Id);
        return l.ReturnAsOk(this);
    }

}