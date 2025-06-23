using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// Official constructor, must call Init afterward
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public sealed class BlockOfModule(BlockGeneratorHelpers services) : BlockOfBase(services, "CB.Mod")
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
        Specs = new()
        {
            Context = ctx,
            AppId = appIdentity.AppId,
            ZoneId = appIdentity.ZoneId,

            IsContentApp = ctx.Module.IsContent,
        };
        Specs = BlockSpecsHelper.CompleteInit(this, Services, null, ctx.Module.BlockIdentifier, ctx.Module.Id, Log);

        Specs = Specs with
        {
            Data = GetData(),
        };

        return l.ReturnAsOk(Specs);
    }

}