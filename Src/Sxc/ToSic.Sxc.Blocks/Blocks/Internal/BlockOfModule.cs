using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// Official constructor, must call Init afterward
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class BlockOfModule(BlockGeneratorHelpers helpers) : ServiceBase("CB.Mod", connect: [helpers])
{
    /// <summary>
    /// Create a module-content block
    /// </summary>
    /// <param name="ctx"></param>
    ///// <param name="overrideParams">optional override parameters</param>
    public IBlock GetBlockOfModule(IContextOfBlock ctx)
    {
        var l = Log.Fn<BlockSpecs>(timer: true);
        var appIdentity = ctx.Module.BlockIdentifier;
        var specs = new BlockSpecs
        {
            Context = ctx,
            AppId = appIdentity.AppId,
            ZoneId = appIdentity.ZoneId,

            IsContentApp = ctx.Module.IsContent,
            IsInnerBlock = false,
        };
        specs = BlockSpecsHelper.CompleteInit(specs, helpers, null, ctx.Module.BlockIdentifier, ctx.Module.Id, Log);

        specs = specs with
        {
            Data = helpers.GetData(specs),
        };

        return l.ReturnAsOk(specs);
    }

}