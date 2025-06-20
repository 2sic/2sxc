using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.PageFeatures;

namespace ToSic.Sxc.Blocks.Internal;
internal class BlockInfoHelpers
{

    internal static void PushAppDependenciesToRoot(IBlock? currentBlock)
    {
        if (currentBlock == null)
            return;

        var myAppId = currentBlock.AppId;
        // this is only relevant for the root builder, so we can skip it for child builders
        if (/*Block == null || Block.*/myAppId == 0)
            return;

        // Cast to current object type to access internal APIs
        if (currentBlock.RootBlock is not { } rootBlock)
            return;

        // add dependent appId only once
        var rootList = rootBlock.DependentApps;
        if (rootList.All(a => a.AppId != myAppId))
            rootList.Add(new DependentApp { AppId = myAppId });
    }

    public static List<IPageFeature> BlockFeatures(IBlock block, ILog log)
        => !block.BlockFeatureKeys.Any()
            ? []
            : ((ContextOfBlock)block.Context).PageServiceShared.PageFeatures.GetWithDependents(block.BlockFeatureKeys, log);

}
