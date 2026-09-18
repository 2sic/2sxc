using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Sys.Render.PageFeatures;

namespace ToSic.Sxc.Blocks.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockFeaturesHelpers
{
    public static List<IPageFeature> BlockFeatures(IBlock block, ILog log)
        => !block.BlockFeatureKeys.Any()
            ? []
            : ((ContextOfBlock)block.Context).PageServiceShared.PageFeatures.GetWithDependents(block.BlockFeatureKeys, log);

}
